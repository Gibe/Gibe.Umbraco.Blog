using Examine;
using Examine.Providers;
using System;
using Gibe.Umbraco.Blog.Extensions;
using Umbraco.Core.Composing;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using Umbraco.Core.Events;
using Gibe.Umbraco.Blog.Exceptions;
using Gibe.Umbraco.Blog.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using Examine.LuceneEngine.Indexing;
using Umbraco.Web;
using Umbraco.Core;
using Examine.LuceneEngine.Providers;

namespace Gibe.Umbraco.Blog.Composing
{
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

			((LuceneIndex)index).FieldDefinitionCollection.AddOrUpdate(new FieldDefinition(ExamineFields.PostDate, FieldDefinitionTypes.DateTime));
			((LuceneIndex)index).FieldDefinitionCollection.AddOrUpdate(new FieldDefinition(ExamineFields.PostDateSort, FieldDefinitionTypes.Long));
			((LuceneIndex)index).FieldDefinitionCollection.AddOrUpdate(new FieldDefinition(ExamineFields.PostDateYear, FieldDefinitionTypes.Raw));
			((LuceneIndex)index).FieldDefinitionCollection.AddOrUpdate(new FieldDefinition(ExamineFields.PostDateMonth, FieldDefinitionTypes.Raw));
			((LuceneIndex)index).FieldDefinitionCollection.AddOrUpdate(new FieldDefinition(ExamineFields.PostDateDay, FieldDefinitionTypes.Raw));
			((LuceneIndex)index).FieldValueTypeCollection.ValueTypeFactories.TryAdd(ExamineFields.Tag,
				name => new RawStringType(ExamineFields.Tag, true));
			((LuceneIndex)index).FieldDefinitionCollection.AddOrUpdate(new FieldDefinition(ExamineFields.Tag, ExamineFields.Tag));
			((LuceneIndex)index).FieldValueTypeCollection.ValueTypeFactories.TryAdd(ExamineFields.CategoryName,
				name => new RawStringType(ExamineFields.CategoryName, true));
			((LuceneIndex)index).FieldDefinitionCollection.AddOrUpdate(new FieldDefinition(ExamineFields.CategoryName, ExamineFields.CategoryName));
			((LuceneIndex)index).FieldValueTypeCollection.ValueTypeFactories.TryAdd(ExamineFields.PostAuthorName,
				name => new RawStringType(ExamineFields.PostAuthorName, true));
			((LuceneIndex)index).FieldDefinitionCollection.AddOrUpdate(new FieldDefinition(ExamineFields.PostAuthorName, ExamineFields.PostAuthorName));
			ContentService.Saving += ContentServiceSaving;
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
			
			document.TryAdd(ExamineFields.PostDateYear, postDate.Year.ToString("00"));
			document.TryAdd(ExamineFields.PostDateMonth, postDate.Month.ToString("00"));
			document.TryAdd(ExamineFields.PostDateDay, postDate.Day.ToString("00"));
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

				var category = context.UmbracoContext.Content.GetById(Udi.Parse(categoryId));
				if (category != null)
				{
					document.TryAddOrAppend(ExamineFields.CategoryName, category.Name);
				}
			}
		}

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

		private string GetUserName(int userId)
		{
			return _userService.GetUserById(userId).Name;
		}

		public void Terminate()
		{

		}
	}
}
