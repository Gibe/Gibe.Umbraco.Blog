using Gibe.Umbraco.Blog.Filters;
using Gibe.Umbraco.Blog.Sort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;

namespace Gibe.Umbraco.Blog.Models
{
	public class BlogCategories : IBlogCategories
	{
		private readonly IBlogSearch _blogSearch;

		public BlogCategories(IBlogSearch blogSearch)
		{
			_blogSearch = blogSearch;
		}

		public IEnumerable<BlogCategory> All(IPublishedContent blogRoot)
		{
			var allCategories = new Dictionary<string, BlogCategory>();
			var posts = _blogSearch.Search(new SectionBlogPostFilter(blogRoot.Id), new DateSort());

			var categories = posts.Where(post => post.Values.ContainsKey($"{ExamineFields.CategoryName}") && 
				!string.IsNullOrEmpty(post.Values[$"{ExamineFields.CategoryName}"]))
					.Select(post => post.Values[$"{ExamineFields.CategoryName}"]);

			foreach (var category in categories)
			{
				if (allCategories.ContainsKey(category))
				{
					allCategories[category].Count++;
					continue;
				}

				allCategories.Add(category, new BlogCategory { Count = 1, Tag = category, Url = $"{blogRoot.Url}?{ExamineFields.Category}={category}" });
			}

			return allCategories.Values;
		}
	}
}
