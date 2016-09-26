﻿using System;
using Examine;
using Examine.SearchCriteria;
using Examine.LuceneEngine;

namespace Gibe.Umbraco.Blog.Wrappers
{
	public interface ISearchIndex
	{
		ISearchCriteria CreateSearchCriteria();
		ISearchResults Search(ISearchCriteria criteria);

		event EventHandler<DocumentWritingEventArgs> DocumentWriting;
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

		public event EventHandler<DocumentWritingEventArgs> DocumentWriting;
	}
}
