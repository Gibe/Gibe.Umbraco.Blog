using System;
using Examine;
using Examine.SearchCriteria;
using Examine.LuceneEngine;
using UmbracoExamine;

namespace Gibe.Umbraco.Blog.Wrappers
{
	public interface ISearchIndex
	{
		ISearchCriteria CreateSearchCriteria();
		ISearchResults Search(ISearchCriteria criteria);

		UmbracoContentIndexer GetIndexer();

	}

	public class FakeSearchIndex : ISearchIndex
	{
		private readonly ISearchResults _searchResults;

		public FakeSearchIndex(ISearchResults searchResults)
		{
			_searchResults = searchResults;
		}

		public ISearchCriteria CreateSearchCriteria()
		{
			return ExamineManager.Instance.CreateSearchCriteria();
		}

		public ISearchResults Search(ISearchCriteria criteria)
		{
			return _searchResults;
		}

		public UmbracoContentIndexer GetIndexer()
		{
			return null;
		}
	}
}
