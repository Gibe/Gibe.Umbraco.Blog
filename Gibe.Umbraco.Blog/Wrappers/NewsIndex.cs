using System;
using Examine;
using Examine.LuceneEngine;
using Examine.SearchCriteria;
using UmbracoExamine;

namespace Gibe.Umbraco.Blog.Wrappers
{
	public class NewsIndex : ISearchIndex
	{
		public event EventHandler<DocumentWritingEventArgs> DocumentWriting;

		public NewsIndex()
		{
			var indexer = (UmbracoContentIndexer)ExamineManager.Instance.IndexProviderCollection["BlogIndexer"];
			indexer.DocumentWriting += DocumentWriting;
		}

		public ISearchCriteria CreateSearchCriteria()
		{
			return ExamineManager.Instance.CreateSearchCriteria();
		}

		public ISearchResults Search(ISearchCriteria criteria)
		{
			return ExamineManager.Instance.SearchProviderCollection["BlogSearcher"].Search(criteria);
		}
	}
}
