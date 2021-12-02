using System.Collections.Generic;
using System.Linq;
using Gibe.Umbraco.Blog.Filters;
using Gibe.Umbraco.Blog.Models;
using Gibe.Umbraco.Blog.Sort;
using Newtonsoft.Json;
using Umbraco.Extensions;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Gibe.Umbraco.Blog
{
	public class BlogTags : IBlogTags
	{
		private readonly IBlogSearch _blogSearch;
		private readonly string _propertyName;
		
		public BlogTags(IBlogSearch blogSearch)
		{
			_blogSearch = blogSearch;
			_propertyName = ExamineFields.Tags;
		}

		public IEnumerable<BlogTag> All(IPublishedContent blogRoot)
		{
			var allTags = new Dictionary<string, BlogTag>();
			var posts = _blogSearch.Search(new SectionBlogPostFilter(blogRoot.Id ), new DateSort());

			var applicablePosts = posts.Where(post => post.Values.ContainsKey($"{_propertyName}") && !string.IsNullOrEmpty(post.Values[$"{_propertyName}"]))
				.SelectMany(post => JsonConvert.DeserializeObject<IEnumerable<string>>(post.Values[$"{_propertyName}"]));

			foreach (var tag in applicablePosts) // TODO not hard coded
			{
				if (allTags.ContainsKey(tag))
				{
					allTags[tag].Count++;
				}
				else
				{
					allTags.Add(tag, new BlogTag { Count = 1, Tag = tag, Url = $"{blogRoot.Url()}?{ExamineFields.Tag}={tag}"});
				}
			}
			return allTags.Values;
		}
	}
}
