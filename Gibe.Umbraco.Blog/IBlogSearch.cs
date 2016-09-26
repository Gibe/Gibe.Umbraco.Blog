using System.Collections.Generic;
using Examine;
using Gibe.Umbraco.Blog.Filters;

namespace Gibe.Umbraco.Blog
{
	public interface IBlogSearch
	{
		ISearchResults Search(IBlogPostFilter filter);
		ISearchResults Search(IBlogPostFilter filter, bool sortByDate);
		ISearchResults Search(IEnumerable<IBlogPostFilter> filters);
		ISearchResults Search(IEnumerable<IBlogPostFilter> filters, bool sortByDate);
	}

	public class FakeBlogSearch : IBlogSearch
	{
		private readonly ISearchResults _searchResults;

		public FakeBlogSearch(ISearchResults searchResults)
		{
			_searchResults = searchResults;
		}


		public ISearchResults Search(IBlogPostFilter filter)
		{
			return _searchResults;
		}

		public ISearchResults Search(IBlogPostFilter filter, bool sortByDate)
		{
			return _searchResults;
		}

		public ISearchResults Search(IEnumerable<IBlogPostFilter> filters)
		{
			return _searchResults;
		}

		public ISearchResults Search(IEnumerable<IBlogPostFilter> filters, bool sortByDate)
		{
			return _searchResults;
		}
	}
}
