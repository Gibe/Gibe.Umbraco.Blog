using System.Collections.Generic;
using Examine;
using Gibe.Umbraco.Blog.Filters;
using Gibe.Umbraco.Blog.Sort;

namespace Gibe.Umbraco.Blog
{
	public interface IBlogSearch
	{
		ISearchResults Search(IBlogPostFilter filter, ISort sort);
		ISearchResults Search(IEnumerable<IBlogPostFilter> filters, ISort sort);
	}

	public class FakeBlogSearch : IBlogSearch
	{
		private readonly ISearchResults _searchResults;

		public FakeBlogSearch(ISearchResults searchResults)
		{
			_searchResults = searchResults;
		}


		public ISearchResults Search(IBlogPostFilter filter, ISort sort)
		{
			return _searchResults;
		}

		public ISearchResults Search(IEnumerable<IBlogPostFilter> filters, ISort sort)
		{
			return _searchResults;
		}
	}
}
