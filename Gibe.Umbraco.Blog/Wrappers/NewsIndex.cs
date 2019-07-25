using Examine;
using Examine.Search;
using Gibe.Umbraco.Blog.Exceptions;

namespace Gibe.Umbraco.Blog.Wrappers
{
	public class NewsIndex : ISearchIndex
	{
		private const string IndexName = "ExternalIndex";

		private readonly IExamineManager _examineManager;

		public NewsIndex(IExamineManager examineManager)
		{
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
			return operation.Execute();
		}

		public IIndex GetIndex()
		{
			IIndex index;

			if (!_examineManager.TryGetIndex(IndexName, out index))
			{
				throw new IndexNotFoundException(IndexName);
			}

			return index;
		}
	}
}
