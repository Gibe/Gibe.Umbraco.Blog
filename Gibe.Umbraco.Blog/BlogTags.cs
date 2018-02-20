using System;
using System.Collections.Generic;
using System.Linq;
using Gibe.Umbraco.Blog.Filters;
using Gibe.Umbraco.Blog.Models;
using Gibe.Umbraco.Blog.Sort;

namespace Gibe.Umbraco.Blog
{
	public class BlogTags : IBlogTags
	{
		private readonly IBlogSearch _blogSearch;

		public BlogTags(IBlogSearch blogSearch)
		{
			_blogSearch = blogSearch;
		}

		public IEnumerable<BlogTag> All(string rootPath)
		{
			var allTags = new Dictionary<string, BlogTag>();
			var posts = _blogSearch.Search(Enumerable.Empty<IBlogPostFilter>(), new DateSort());
			foreach (var tag in posts.Where(post => post.Fields.ContainsKey("settingsNewsTags") && !String.IsNullOrEmpty(post.Fields["settingsNewsTags"])).SelectMany(post => post.Fields["settingsNewsTags"].Split(','))) // TODO not hard coded
			{
				if (allTags.ContainsKey(tag))
				{
					allTags[tag].Count++;
				}
				else
				{
					allTags.Add(tag, new BlogTag { Count = 1, Tag = tag, Url = $"{rootPath}?tag={tag}"});
				}
			}
			return allTags.Values;
		}
	}
}
