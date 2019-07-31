using Examine;
using Examine.Search;
using Gibe.Umbraco.Blog.Exceptions;
using Gibe.Umbraco.Blog.Models;

namespace Gibe.Umbraco.Blog.Wrappers
{
	public class NewsIndex : ISearchIndex
	{
		private readonly IExamineManager _examineManager;
		private readonly IBlogSettings _blogSettings;

		public NewsIndex(IExamineManager examineManager,
			IBlogSettings blogSettings)
		{
			_examineManager = examineManager;
			_blogSettings = blogSettings;
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

			if (!_examineManager.TryGetIndex(_blogSettings.IndexName, out index))
			{
				throw new IndexNotFoundException(_blogSettings.IndexName);
			}

			return index;
		}
	}
}
