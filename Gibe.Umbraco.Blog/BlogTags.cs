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
        private readonly string _propertyName = ExamineFields.Tags;


        public BlogTags(IBlogSearch blogSearch)
		{
			_blogSearch = blogSearch;
		}

		public IEnumerable<BlogTag> All(IPublishedContent blogRoot)
		{
			var allTags = new Dictionary<string, BlogTag>();
			var posts = _blogSearch.Search(new SectionBlogPostFilter(blogRoot.Id ), new DateSort());

			var applicablePosts = posts.Where(post => post.Values.ContainsKey($"{_propertyName}") && !string.IsNullOrEmpty(post.Values[$"{_propertyName}"]))
				.SelectMany(post => ParseTags(post.Values[$"{_propertyName}"]));

			foreach (var tag in applicablePosts)
			{
				if (allTags.TryGetValue(tag, out var allTag))
				{
					allTag.Count++;
				}
				else
				{
					allTags.Add(tag, new BlogTag { Count = 1, Tag = tag, Url = $"{blogRoot.Url()}?{ExamineFields.Tag}={tag}"});
				}
			}
			return allTags.Values;
		}

		private static IEnumerable<string> ParseTags(string rawValue)
		{
			try
			{
				return JsonConvert.DeserializeObject<IEnumerable<string>>(rawValue) ?? Enumerable.Empty<string>();
			}
			catch (JsonReaderException)
			{
				// Tags are likely csv rather than JSON
				return rawValue.Split(',');
			}
		}
	}
}
