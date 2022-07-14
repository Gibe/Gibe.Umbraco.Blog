using Examine;
using System;
using Gibe.Umbraco.Blog.Extensions;
using Gibe.Umbraco.Blog.Exceptions;
using Gibe.Umbraco.Blog.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Examine.Lucene.Indexing;
using Examine.Lucene;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;


namespace Gibe.Umbraco.Blog.Composing
{

	public class AddFieldsToExternalIndex : IConfigureNamedOptions<LuceneDirectoryIndexOptions>
	{
		private readonly ILoggerFactory _logger;

		public AddFieldsToExternalIndex(ILoggerFactory logger)
		{
			_logger = logger;
		}

		public void Configure(LuceneDirectoryIndexOptions options)
			=> throw new NotImplementedException("This is never called and is just part of the interface");


		public void Configure(string name, LuceneDirectoryIndexOptions options)
		{
			switch (name)
			{
				case Constants.UmbracoIndexes.ExternalIndexName:
					options.FieldDefinitions.AddOrUpdate(new FieldDefinition(ExamineFields.PostDate, FieldDefinitionTypes.DateTime));
					options.FieldDefinitions.AddOrUpdate(new FieldDefinition(ExamineFields.PostDateYear, FieldDefinitionTypes.DateYear));
					options.FieldDefinitions.AddOrUpdate(new FieldDefinition(ExamineFields.PostDateMonth, FieldDefinitionTypes.DateMonth));
					options.FieldDefinitions.AddOrUpdate(new FieldDefinition(ExamineFields.PostDateDay, FieldDefinitionTypes.DateDay));
					options.FieldDefinitions.AddOrUpdate(new FieldDefinition(ExamineFields.Tag, ExamineFields.Tag));
					options.FieldDefinitions.AddOrUpdate(new FieldDefinition(ExamineFields.CategoryName, ExamineFields.CategoryName));
					options.FieldDefinitions.AddOrUpdate(new FieldDefinition(ExamineFields.PostAuthorName, ExamineFields.PostAuthorName));

					options.IndexValueTypesFactory = new Dictionary<string, IFieldValueTypeFactory>
					{
						[ExamineFields.Tag] = new DelegateFieldValueTypeFactory(n =>
							new RawStringType(n, _logger, true)),
						[ExamineFields.CategoryName] = new DelegateFieldValueTypeFactory(n =>
							new RawStringType(n, _logger, true)),
						[ExamineFields.PostAuthorName] = new DelegateFieldValueTypeFactory(n =>
							new RawStringType(n, _logger, true)),

					};
					break;
			}
		}
	}

	public class IndexEventsComposer : IComposer
	{
		public void Compose(IUmbracoBuilder builder)
		{
			builder.Components().Append<IndexEventsComponent>();
		}
	}

	public class IndexEventsComponent : IComponent
	{
		private readonly IExamineManager _examineManager;
		private readonly IUserService _userService;
		private readonly IBlogSettings _blogSettings;
		private readonly IUmbracoContextFactory _umbracoContextFactory;

		public IndexEventsComponent(IExamineManager examineManager,
			IUserService userService,
			IBlogSettings blogSettings,
			IUmbracoContextFactory umbracoContextFactory)
		{
			_examineManager = examineManager;
			_userService = userService;
			_blogSettings = blogSettings;
			_umbracoContextFactory = umbracoContextFactory;
		}

		public void Initialize()
		{
			_examineManager.TryGetIndex(_blogSettings.IndexName, out var index);

			if (index == null)
			{
				throw new IndexNotFoundException(_blogSettings.IndexName);
			}

			((BaseIndexProvider)index).TransformingIndexValues += ExternalIndexTransformingIndexValues;
		}

		private void ExternalIndexTransformingIndexValues(object sender, IndexingItemEventArgs e)
		{
			var document = e.ValueSet;

			if (document.ItemType != _blogSettings.BlogPostDocumentTypeAlias)
			{
				return;
			}

			IDictionary<string, IEnumerable<object>>
				values = e.ValueSet.Values.ToDictionary(x => x.Key, x => x.Value.ToList().AsEnumerable());
			
			values.MergeLeft(AddPostDateFields(e));
			values.MergeLeft(AddAuthorFields(e));
			values.MergeLeft(AddTagFields(e));
			values.MergeLeft(AddPathFields(e));
			values.MergeLeft(AddCategoryFields(e));
			e.SetValues(values);
		}

		private IDictionary<string, IEnumerable<object>> AddPostDateFields(IndexingItemEventArgs e)
		{
			var postDate = e.ValueSet.GetSingleValue<DateTime>(ExamineFields.PostDate);
			Dictionary<string, IEnumerable<object>> values = new Dictionary<string, IEnumerable<object>>();
			
			values.Add(ExamineFields.PostDateYear, new [] { postDate.Year.ToString()});
			values.Add(ExamineFields.PostDateMonth, new[] { postDate.Month.ToString() });
			values.Add(ExamineFields.PostDateDay, new[] { postDate.Day.ToString() });
			return values;
		}

		private Dictionary<string, IEnumerable<object>> AddAuthorFields(IndexingItemEventArgs e)
		{
			var authorId = e.ValueSet.GetSingleValue<int?>(ExamineFields.PostAuthor);
			Dictionary<string, IEnumerable<object>> values = new Dictionary<string, IEnumerable<object>>();

			if (authorId.HasValue)
			{
				values.Add(ExamineFields.PostAuthorName, new[] { GetUserName(authorId.Value).ToLower()});
			}

			return values;
		}

		private Dictionary<string, IEnumerable<object>> AddTagFields(IndexingItemEventArgs e)
		{
			Dictionary<string, IEnumerable<object>> values = new Dictionary<string, IEnumerable<object>>();
			var tagValue = e.ValueSet.GetSingleValue(ExamineFields.Tags);
			if (tagValue != null)
			{
				try
				{
					var tags =
						JsonConvert.DeserializeObject<IEnumerable<string>>(tagValue);
					if (tags != null)
					{
						values.Add(ExamineFields.Tag, tags.Select(t => t.ToLower()));
					}
				}
				catch (JsonReaderException)
				{
					// Tags are invalid
					var tags = tagValue.Split(",");
					values.Add(ExamineFields.Tag, tags.Select(t => t.ToLower()));
				}
			}
			return values;
		}

		private Dictionary<string, IEnumerable<object>> AddPathFields(IndexingItemEventArgs e)
		{
			Dictionary<string, IEnumerable<object>> values = new Dictionary<string, IEnumerable<object>>();
			var path = e.ValueSet.GetSingleValue(ExamineFields.Path);
			if (path != null)
			{
				values.Add(ExamineFields.Path, path.Split(','));
			}
			return values;
		}

		private Dictionary<string, IEnumerable<object>> AddCategoryFields(IndexingItemEventArgs e)
		{
			Dictionary<string, IEnumerable<object>> values = new Dictionary<string, IEnumerable<object>>();
			using (var context = _umbracoContextFactory.EnsureUmbracoContext())
			{
				var categoryId = e.ValueSet.GetSingleValue(ExamineFields.Category);

				if (string.IsNullOrEmpty(categoryId))
				{
					return values;
				}

				var udi = UdiParser.Parse(categoryId);
				var category = context.UmbracoContext.Content.GetById(udi);
				if (category != null)
				{
					values.Add(ExamineFields.CategoryName, new[] { category.Name });
				}
			}
			return values;
		}

		private string GetUserName(int userId)
		{
			return _userService.GetUserById(userId).Name;
		}

		public void Terminate()
		{

		}
	}
}
