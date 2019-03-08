using System;
using System.Collections.Generic;
using System.Linq;
using Gibe.Umbraco.Blog.Filters;
using Gibe.Umbraco.Blog.Models;
using Gibe.Umbraco.Blog.Sort;
using Umbraco.Core.Models;

namespace Gibe.Umbraco.Blog
{
	public class BlogTags : IBlogTags
	{
		private readonly IBlogSearch _blogSearch;
		private readonly string _propertyName;
		private readonly string _queryStringName;
		
		public BlogTags(IBlogSearch blogSearch, string propertyName = "settingsNewsTags", string querystringName = "tag")
		{
			_blogSearch = blogSearch;
			_propertyName = propertyName;
			_queryStringName = querystringName;
		}

		public IEnumerable<BlogTag> All(IPublishedContent blogRoot)
		{
			var allTags = new Dictionary<string, BlogTag>();
			var posts = _blogSearch.Search(new SectionBlogPostFilter(blogRoot.Id ), new DateSort());
			foreach (var tag in posts.Where(post => post.Fields.ContainsKey($"{_propertyName}") && !String.IsNullOrEmpty(post.Fields[$"{_propertyName}"])).SelectMany(post => post.Fields[$"{_propertyName}"].Split(','))) // TODO not hard coded
			{
				if (allTags.ContainsKey(tag))
				{
					allTags[tag].Count++;
				}
				else
				{
					allTags.Add(tag, new BlogTag { Count = 1, Tag = tag, Url = $"{blogRoot.Url}?{_queryStringName}={tag}"});
				}
			}
			return allTags.Values;
		}
	}
}
