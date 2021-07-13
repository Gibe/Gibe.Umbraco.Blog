using Examine;
using System;
using Gibe.Umbraco.Blog.Extensions;
using Gibe.Umbraco.Blog.Exceptions;
using Gibe.Umbraco.Blog.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

#if NET5_0
using Examine.Lucene;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Microsoft.Extensions.Options;
#elif NET472
using Examine.Providers;
using Umbraco.Core.Composing;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using Umbraco.Core.Events;

using Examine.LuceneEngine.Indexing;
using Umbraco.Web;
using Umbraco.Core;
using Examine.LuceneEngine.Providers;
#endif

namespace Gibe.Umbraco.Blog.Composing
{

#if NET5_0
	public class AddFieldsToExternalIndex : IConfigureNamedOptions<LuceneDirectoryIndexOptions>
	{
		public void Configure(LuceneDirectoryIndexOptions options)
		{
			throw new NotImplementedException();
		}

		public void Configure(string name, LuceneDirectoryIndexOptions options)
		{
			switch (name)
			{
				case Constants.UmbracoIndexes.ExternalIndexName:
					options.FieldDefinitions.AddOrUpdate(new FieldDefinition(ExamineFields.PostDate, FieldDefinitionTypes.DateTime));
					options.FieldDefinitions.AddOrUpdate(new FieldDefinition(ExamineFields.PostDateSort, FieldDefinitionTypes.Long));
					options.FieldDefinitions.AddOrUpdate(new FieldDefinition(ExamineFields.PostDateYear, FieldDefinitionTypes.DateYear));
					options.FieldDefinitions.AddOrUpdate(new FieldDefinition(ExamineFields.PostDateMonth, FieldDefinitionTypes.DateMonth));
					options.FieldDefinitions.AddOrUpdate(new FieldDefinition(ExamineFields.PostDateDay, FieldDefinitionTypes.DateDay));
					/*((LuceneIndex)index).FieldValueTypeCollection.ValueTypeFactories.TryAdd(ExamineFields.Tag,
						name => new RawStringType(ExamineFields.Tag, true));*/
					options.FieldDefinitions.AddOrUpdate(new FieldDefinition(ExamineFields.Tag, ExamineFields.Tag));
					/*((LuceneIndex)index).FieldValueTypeCollection.ValueTypeFactories.TryAdd(ExamineFields.CategoryName,
						name => new RawStringType(ExamineFields.CategoryName, true));*/
					options.FieldDefinitions.AddOrUpdate(new FieldDefinition(ExamineFields.CategoryName, ExamineFields.CategoryName));
					/*((LuceneIndex)index).FieldValueTypeCollection.ValueTypeFactories.TryAdd(ExamineFields.PostAuthorName,
						name => new RawStringType(ExamineFields.PostAuthorName, true));*/
					options.FieldDefinitions.AddOrUpdate(new FieldDefinition(ExamineFields.PostAuthorName, ExamineFields.PostAuthorName));
					//ContentService.Saving += ContentServiceSaving;
					break;
			}
		}
	}
#endif

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

#if NET472
			LuceneIndex luceneIndex = (LuceneIndex) index;
			luceneIndex.FieldDefinitionCollection.AddOrUpdate(new FieldDefinition(ExamineFields.PostDate, FieldDefinitionTypes.DateTime));
			luceneIndex.FieldDefinitionCollection.AddOrUpdate(new FieldDefinition(ExamineFields.PostDateSort, FieldDefinitionTypes.Long));
			luceneIndex.FieldDefinitionCollection.AddOrUpdate(new FieldDefinition(ExamineFields.PostDateYear, FieldDefinitionTypes.DateYear));
			luceneIndex.FieldDefinitionCollection.AddOrUpdate(new FieldDefinition(ExamineFields.PostDateMonth, FieldDefinitionTypes.DateMonth));
			luceneIndex.FieldDefinitionCollection.AddOrUpdate(new FieldDefinition(ExamineFields.PostDateDay, FieldDefinitionTypes.DateDay));
			luceneIndex.FieldValueTypeCollection.ValueTypeFactories.TryAdd(ExamineFields.Tag,
				name => new RawStringType(ExamineFields.Tag, true));
			luceneIndex.FieldDefinitionCollection.AddOrUpdate(new FieldDefinition(ExamineFields.Tag, ExamineFields.Tag));
			luceneIndex.FieldValueTypeCollection.ValueTypeFactories.TryAdd(ExamineFields.CategoryName,
				name => new RawStringType(ExamineFields.CategoryName, true));
			luceneIndex.FieldDefinitionCollection.AddOrUpdate(new FieldDefinition(ExamineFields.CategoryName, ExamineFields.CategoryName));
			luceneIndex.FieldValueTypeCollection.ValueTypeFactories.TryAdd(ExamineFields.PostAuthorName,
				name => new RawStringType(ExamineFields.PostAuthorName, true));
			luceneIndex.FieldDefinitionCollection.AddOrUpdate(new FieldDefinition(ExamineFields.PostAuthorName, ExamineFields.PostAuthorName));
			ContentService.Saving += ContentServiceSaving;
#endif
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
			document.TryAdd(ExamineFields.PostDateSort, postDate.Ticks);
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

#if NET5_0
				var udi = UdiParser.Parse(categoryId);
#elif NET472
				var udi = Udi.Parse(categoryId);
#endif
				var category = context.UmbracoContext.Content.GetById(udi);
				if (category != null)
				{
					document.TryAddOrAppend(ExamineFields.CategoryName, category.Name);
				}
			}
		}
#if NET472
		private void ContentServiceSaving(IContentService sender, ContentSavingEventArgs e)
		{
			foreach (var entity in e.SavedEntities)
			{
				try
				{
					if (entity.ContentType.Alias != _blogSettings.BlogPostDocumentTypeAlias || entity.ParentId == -20)
					{
						continue;
					}

					var parentContent = sender.GetById(entity.ParentId);
					if (parentContent.Published)
					{
						//if the date hasn't been set, default it to today
						var postDateString = entity.GetValue<string>(ExamineFields.PostDate);
						if (string.IsNullOrEmpty(postDateString))
						{
							entity.SetValue(ExamineFields.PostDate, DateTime.Now.Date);
						}
					}
				}
				catch (InvalidOperationException)
				{
					// This happens if you try to get ParentId during install of a package with content
				}
			}
		}
#endif

		private string GetUserName(int userId)
		{
			return _userService.GetUserById(userId).Name;
		}

		public void Terminate()
		{

		}
	}
}
