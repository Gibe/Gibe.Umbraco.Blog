using Gibe.Umbraco.Blog.Filters;
using Gibe.Umbraco.Blog.Models;
using Gibe.Umbraco.Blog.Sort;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Gibe.Umbraco.Blog
{
	public class UnpagedBlogService<T> : IUnpagedBlogService<T> where T : class, IBlogPostModel
	{
		private readonly IBlogSearch _blogSearch;
		private readonly IBlogPostMapper<T> _blogPostMapper;

		public UnpagedBlogService(
			IBlogSearch blogSearch,
			IBlogPostMapper<T> blogPostMapper)
		{
			_blogSearch = blogSearch;
			_blogPostMapper = blogPostMapper;
		}

		public UnpagedBlogSearchResults<T> GetPosts(int startPost, int postCount, ISort sort = null,
			IEnumerable<IBlogPostFilter> filters = null)
		{
			if (sort == null) sort = new DateSort();
			if (filters == null) filters = Enumerable.Empty<IBlogPostFilter>();

			var results = _blogSearch.Search(filters, sort);

			return new UnpagedBlogSearchResults<T>
			{
				BlogPosts = _blogPostMapper.ToBlogPosts(results.Skip(startPost - 1).Take(postCount), new NoopPublishedValueFallback()),
				TotalItemsCount = results.Count()
			};
		}
	}

	public class UnpagedBlogSearchResults<T> where T : class, IBlogPostModel
	{
		public IEnumerable<T> BlogPosts { get; set; }
		public int TotalItemsCount { get; set; }
	}
}
