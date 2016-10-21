using System;
using Examine;
using Examine.LuceneEngine;
using Examine.Providers;
using Examine.SearchCriteria;
using UmbracoExamine;

namespace Gibe.Umbraco.Blog.Wrappers
{
	public class NewsIndex : ISearchIndex
	{
		public UmbracoContentIndexer GetIndexer()
		{
			return (UmbracoContentIndexer)ExamineManager.Instance.IndexProviderCollection["BlogIndexer"];
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
