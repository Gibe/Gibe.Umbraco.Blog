using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
		private IEnumerable<IBlogPostFilter> _filters;

		public FakeBlogSearch(ISearchResults searchResults)
		{
			_searchResults = searchResults;
		}
		
		public ISearchResults Search(IBlogPostFilter filter, ISort sort)
		{
			_filters = new []{filter};
			return _searchResults;
		}

		public ISearchResults Search(IEnumerable<IBlogPostFilter> filters, ISort sort)
		{
			_filters = filters;
			return _searchResults;
		}

		public IEnumerable<IBlogPostFilter> LastUsedFilters => _filters;
	}

	public class FakeSearchResults : ISearchResults
	{
		private readonly IEnumerable<SearchResult> _results;

		public FakeSearchResults(IEnumerable<SearchResult> results)
		{
			_results = results;
		}

		public IEnumerator<ISearchResult> GetEnumerator()
			=> _results.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> _results.GetEnumerator();

		public IEnumerable<ISearchResult> Skip(int skip)
			=> _results.Skip(skip);

		long ISearchResults.TotalItemCount =>
			_results.Count();
	}
}
