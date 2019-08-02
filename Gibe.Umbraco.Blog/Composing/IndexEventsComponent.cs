﻿using Examine;
using Examine.Providers;
using System;
using Gibe.Umbraco.Blog.Extensions;
using Umbraco.Core.Composing;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using Umbraco.Core.Events;
using Gibe.Umbraco.Blog.Exceptions;
using Gibe.Umbraco.Blog.Extensions;
using Gibe.Umbraco.Blog.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gibe.Umbraco.Blog.Composing
{
	public class IndexEventsComponent : IComponent
	{
		private readonly IExamineManager _examineManager;
		private readonly IUserService _userService;
		private readonly IBlogSettings _blogSettings;

		public IndexEventsComponent(IExamineManager examineManager,
			IUserService userService,
			IBlogSettings blogSettings)
		{
			_examineManager = examineManager;
			_userService = userService;
			_blogSettings = blogSettings;
		}

		public void Initialize()
		{
			_examineManager.TryGetIndex(_blogSettings.IndexName, out var index);

			if (index == null)
			{
				throw new IndexNotFoundException(_blogSettings.IndexName);
			}

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

			var postDate = document.GetSingleValue(ExamineFields.PostDate).ParseFromExamineField();

			document.TryAdd(ExamineFields.PostDateYear, postDate.Year.ToString("0000"));
			document.TryAdd(ExamineFields.PostDateMonth, postDate.Month.ToString("00"));
			document.TryAdd(ExamineFields.PostDateDay, postDate.Day.ToString("00"));

			var authorId = document.GetSingleValue<int?>(ExamineFields.PostAuthor);

			if (authorId.HasValue)
			{
				document.TryAdd(ExamineFields.PostAuthorName, GetUserName(authorId.Value).ToLower());
			}

			var tagValue = document.GetSingleValue(ExamineFields.NewsTags);
			if (tagValue != null)
			{
				var tags = JsonConvert.DeserializeObject<IEnumerable<string>>(tagValue);

				foreach (var tag in tags)
				{
					document.TryAddOrAppend(ExamineFields.Tag, tag.ToLower());
				}
			}

			var path = document.GetSingleValue(ExamineFields.Path);
			if (path != null)
			{
				foreach (var id in path.Split(','))
				{
					document.TryAddOrAppend(ExamineFields.Path, id);
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