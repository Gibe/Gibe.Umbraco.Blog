using Examine;
using System;
using Gibe.Umbraco.Blog.Extensions;
using Gibe.Umbraco.Blog.Exceptions;
using Gibe.Umbraco.Blog.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using Examine.Lucene.Indexing;
using Examine.Lucene;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.DependencyInjection;


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

			AddPostDateFields(document);
			AddAuthorFields(document);
			AddTagFields(document);
			AddPathFields(document);
			AddCategoryFields(document);
		}

		private void AddPostDateFields(ValueSet document)
		{
			var postDate = document.GetSingleValue<DateTime>(ExamineFields.PostDate);

			document.TryAdd(ExamineFields.PostDateYear, postDate);
			document.TryAdd(ExamineFields.PostDateMonth, postDate);
			document.TryAdd(ExamineFields.PostDateDay, postDate);
		}

		private void AddAuthorFields(ValueSet document)
		{
			var authorId = document.GetSingleValue<int?>(ExamineFields.PostAuthor);

			if (authorId.HasValue)
			{
				document.TryAdd(ExamineFields.PostAuthorName, GetUserName(authorId.Value).ToLower());
			}
		}

		private void AddTagFields(ValueSet document)
		{
			var tagValue = document.GetSingleValue(ExamineFields.Tags);
			if (tagValue != null)
			{
				try
				{
					var tags =
						JsonConvert.DeserializeObject<IEnumerable<string>>(tagValue);

					foreach (var tag in tags)
					{
						document.TryAddOrAppend(ExamineFields.Tag, tag.ToLower());
					}
				}
				catch (JsonReaderException)
				{
					// Tags are invalid
					var tags = tagValue.Split(",");
					foreach (var tag in tags)
					{
						document.TryAddOrAppend(ExamineFields.Tag, tag.ToLower());
					}
				}
			}
		}

		private void AddPathFields(ValueSet document)
		{
			var path = document.GetSingleValue(ExamineFields.Path);
			if (path != null)
			{
				foreach (var id in path.Split(','))
				{
					document.TryAddOrAppend(ExamineFields.Path, id);
				}
			}
		}

		private void AddCategoryFields(ValueSet document)
		{
			using (var context = _umbracoContextFactory.EnsureUmbracoContext())
			{
				var categoryId = document.GetSingleValue(ExamineFields.Category);

				if (string.IsNullOrEmpty(categoryId))
				{
					return;
				}

				var udi = UdiParser.Parse(categoryId);
				var category = context.UmbracoContext.Content.GetById(udi);
				if (category != null)
				{
					document.TryAddOrAppend(ExamineFields.CategoryName, category.Name);
				}
			}
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
