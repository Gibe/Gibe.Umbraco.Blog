using System;
using System.Collections.Generic;
using System.Linq;
using Gibe.Umbraco.Blog.Filters;
using Gibe.Umbraco.Blog.Models;

namespace Gibe.Umbraco.Blog
{
	public class BlogTags : IBlogTags
	{
		private readonly IBlogSearch _blogSearch;

		public BlogTags(IBlogSearch blogSearch)
		{
			_blogSearch = blogSearch;
		}

		public IEnumerable<BlogTag> All()
		{
			var allTags = new Dictionary<string, BlogTag>();
			var posts = _blogSearch.Search(Enumerable.Empty<IBlogPostFilter>());
			foreach (var tag in posts.Where(post => !String.IsNullOrEmpty(post.Fields["settingsNewsTags"])).SelectMany(post => post.Fields["settingsNewsTags"].Split(','))) // TODO not hard coded
			{
				if (allTags.ContainsKey(tag))
				{
					allTags[tag].Count++;
				}
				else
				{
					allTags.Add(tag, new BlogTag { Count = 1, Tag = tag});
				}
			}
			return allTags.Values;
		}
	}
}
