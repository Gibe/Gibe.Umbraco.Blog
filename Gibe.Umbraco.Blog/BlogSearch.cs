using System.Collections.Generic;
using Examine;
using Examine.Search;
using Gibe.Umbraco.Blog.Filters;
using Gibe.Umbraco.Blog.Sort;
using Gibe.Umbraco.Blog.Wrappers;
using Umbraco.Examine;

namespace Gibe.Umbraco.Blog
{
	public class BlogSearch : IBlogSearch
	{
		private const string BlogPostDoctype = "blogPost";

		private readonly ISearchIndex _newsIndex;

		public BlogSearch(ISearchIndex newsIndex)
		{
			_newsIndex = newsIndex;
		}
		
		public ISearchResults Search(IBlogPostFilter filter, ISort sort)
		{
			return Search(new List<IBlogPostFilter> { filter }, sort);
		}
		
		public ISearchResults Search(IEnumerable<IBlogPostFilter> filters, ISort sort)
		{
			return SearchForBlogPosts(GetSearchQuery(filters, sort));
		}
		
		private IOrdering GetSearchQuery(IEnumerable<IBlogPostFilter> filters, ISort sort)
		{		
			return sort.GetCriteria(GetQuery(filters));
		}

		private IBooleanOperation GetQuery(IEnumerable<IBlogPostFilter> filters)
		{
			var query = _newsIndex.CreateSearchQuery().NodeTypeAlias(BlogPostDoctype);
			foreach (var filter in filters)
			{
				query = filter.GetCriteria(query.And());
			}
			return query;
		}

		private ISearchResults SearchForBlogPosts(IOrdering operation)
		{
			return operation.Execute();
		}
	}
	
}
