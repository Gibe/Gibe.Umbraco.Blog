using Examine;
using Examine.Search;
using System.Linq;

namespace Gibe.Umbraco.Blog.Wrappers
{
	public interface ISearchIndex
	{
		IQuery CreateSearchQuery();

		ISearchResults Search(IBooleanOperation operation);

		IIndex GetIndex();
	}

	public class FakeSearchIndex : ISearchIndex
	{
		private readonly ISearchResults _searchResults;
		private readonly IExamineManager _examineManager;

		public FakeSearchIndex(ISearchResults searchResults,
			IExamineManager examineManager)
		{
			_searchResults = searchResults;
			_examineManager = examineManager;
		}

		public IQuery CreateSearchQuery()
		{
			return GetIndex()
				.GetSearcher()
				.CreateQuery();
		}

		public ISearchResults Search(IBooleanOperation operation)
		{
			return _searchResults;
		}

		public IIndex GetIndex()
		{
			return _examineManager
				.Indexes
				.First();
		}
	}

#if NET6
	public static class IndexExtensions
	{
		public  static ISearcher GetSearcher(this IIndex index)
		{
			return index.Searcher;
		}
	}
#endif
}
