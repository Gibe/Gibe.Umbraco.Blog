using System.Collections.Generic;
using Gibe.Umbraco.Blog.Filters;
using Gibe.Umbraco.Blog.Models;
using Gibe.Umbraco.Blog.Sort;

namespace Gibe.Umbraco.Blog
{
	public interface IUnpagedBlogService<T> where T : class, IBlogPostModel
	{
		UnpagedBlogSearchResults<T> GetPosts(int startPost, int endPost, ISort sort = null, IEnumerable<IBlogPostFilter> filters = null);
	}
}
