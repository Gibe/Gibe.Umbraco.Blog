using System.Collections.Generic;
using System.Linq;
using Gibe.Umbraco.Blog.Filters;
using Gibe.Umbraco.Blog.Models;
using Gibe.Umbraco.Blog.Sort;
using Umbraco.Core.Models.PublishedContent;

namespace Gibe.Umbraco.Blog
{
	public class BlogTags : IBlogTags
	{
		private readonly IBlogSearch _blogSearch;
		private readonly string _propertyName;
		private readonly string _queryStringName;
		
		public BlogTags(IBlogSearch blogSearch, string propertyName = null, string querystringName = null)
		{
			_blogSearch = blogSearch;
			_propertyName = propertyName ?? ExamineFields.NewsTags;
			_queryStringName = querystringName ?? ExamineFields.Tag;
		}

		public IEnumerable<BlogTag> All(IPublishedContent blogRoot)
		{
			var allTags = new Dictionary<string, BlogTag>();
			var posts = _blogSearch.Search(new SectionBlogPostFilter(blogRoot.Id ), new DateSort());

			var applicablePosts = posts.Where(post => post.Values.ContainsKey($"{_propertyName}") && !string.IsNullOrEmpty(post.Values[$"{_propertyName}"]))
				.SelectMany(post => post.Values[$"{_propertyName}"].Split(','));

			foreach (var tag in applicablePosts) // TODO not hard coded
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
