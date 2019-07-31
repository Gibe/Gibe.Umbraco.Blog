using Examine;
using Examine.Providers;
using System;
using System.Globalization;
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

			var postDate = DateTime.ParseExact(document.GetSingleValue("postDate").Substring(0, 10), "MM/dd/yyyy", CultureInfo.InvariantCulture);
			document.TryAdd("postDateYear", postDate.Year.ToString("0000"));
			document.TryAdd("postDateMonth", postDate.Month.ToString("00"));
			document.TryAdd("postDateDay", postDate.Day.ToString("00"));

			var authorId = document.GetSingleValue<int?>("postAuthor");

			if (authorId.HasValue)
			{
				document.TryAdd("postAuthorName", GetUserName(authorId.Value).ToLower());
			}

			var tagValue = document.GetSingleValue("settingsNewsTags");
			if (tagValue != null)
			{
				var tags = JsonConvert.DeserializeObject<IEnumerable<string>>(tagValue);

				foreach (var tag in tags)
				{
					document.TryAddOrAppend("tag", tag.ToLower());
				}
			}

			var path = document.GetSingleValue("path");
			if (path != null)
			{
				foreach (var id in path.Split(','))
				{
					document.TryAddOrAppend("path", id);
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

					// TODO : Move code to somewhere better
					var parentContent = sender.GetById(entity.ParentId);
					if (parentContent.Published)
					{
						//if the date hasn't been set, default it to today
						var postDate = DateTime.Now.Date;
						var postDateString = entity.GetValue<string>("postDate");
						if (string.IsNullOrEmpty(postDateString))
						{
							entity.SetValue("postDate", postDate);
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
