using System.Collections.Generic;
using Examine;
using Examine.LuceneEngine.SearchCriteria;
using Examine.SearchCriteria;
using Gibe.Umbraco.Blog.Filters;
using Gibe.Umbraco.Blog.Wrappers;

namespace Gibe.Umbraco.Blog
{
	public class BlogSearch : IBlogSearch
	{
		private const string BlogPostDoctype = "blogPost";

		private ISearchIndex _newsIndex;

		public BlogSearch(ISearchIndex newsIndex)
		{
			_newsIndex = newsIndex;
		}

		public ISearchResults Search(IBlogPostFilter filter)
		{
			return Search(new List<IBlogPostFilter> {filter});
		}

		public ISearchResults Search(IBlogPostFilter filter, bool sortByDate)
		{
			return Search(new List<IBlogPostFilter> { filter }, sortByDate);
		}

		public ISearchResults Search(IEnumerable<IBlogPostFilter> filters)
		{
			return SearchForBlogPosts(GetSearchQuery(filters));
		}

		public ISearchResults Search(IEnumerable<IBlogPostFilter> filters, bool sortByDate)
		{
			return SearchForBlogPosts(GetSearchQuery(filters, sortByDate));
		}

		private ISearchCriteria GetSearchQuery(IEnumerable<IBlogPostFilter> filters)
		{
			return GetQuery(filters).Compile();
		}

		private ISearchCriteria GetSearchQuery(IEnumerable<IBlogPostFilter> filters, bool sortByDate)
		{
			if (sortByDate)
			{
				return GetQuery(filters).And().OrderByDescending(new SortableField("postDate", SortType.String)).Compile();
			}
			return GetSearchQuery(filters);
		}

		private IBooleanOperation GetQuery(IEnumerable<IBlogPostFilter> filters)
		{
			var query = _newsIndex.CreateSearchCriteria().NodeTypeAlias(BlogPostDoctype);
			foreach (var filter in filters)
			{
				query = filter.GetCriteria(query.And());
			}
			return query;
		}

		private ISearchResults SearchForBlogPosts(ISearchCriteria query)
		{
			return _newsIndex.Search(query);
		}
	}
	
}
