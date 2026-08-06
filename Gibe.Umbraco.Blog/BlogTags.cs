using System.Collections.Generic;
using System.Linq;
using Gibe.Umbraco.Blog.Filters;
using Gibe.Umbraco.Blog.Models;
using Gibe.Umbraco.Blog.Sort;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Gibe.Umbraco.Blog
{
	public class BlogTags : IBlogTags
	{
		private readonly IBlogSearch _blogSearch;
		private readonly string _propertyName = ExamineFields.Tags;


		public BlogTags(IBlogSearch blogSearch)
		{
			_blogSearch = blogSearch;
		}

		public IEnumerable<BlogTag> All(IPublishedContent blogRoot)
		{
			var allTags = new Dictionary<string, BlogTag>();
			var posts = _blogSearch.Search(new SectionBlogPostFilter(blogRoot.Id), new DateSort());

			var applicablePosts = posts.Where(post => post.Values.ContainsKey($"{_propertyName}") && !string.IsNullOrEmpty(post.Values[$"{_propertyName}"]));
			var tagSets = applicablePosts.Select(posts => posts.AllValues[$"{_propertyName}"]);

			foreach (var tagSet in tagSets)
			{
				foreach (var tag in tagSet)
				{
					if (allTags.TryGetValue(tag, out var allTag))
					{
						allTag.Count++;
					}
					else
					{
						allTags.Add(tag,
								new BlogTag { Count = 1, Tag = tag, Url = $"{blogRoot.Url()}?{ExamineFields.Tag}={tag}" });
					}
				}
			}
			return allTags.Values.OrderByDescending(t => t.Count);
		}
	}
}
