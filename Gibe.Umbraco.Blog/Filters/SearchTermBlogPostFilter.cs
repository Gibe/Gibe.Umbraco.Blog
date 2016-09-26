﻿using Examine.SearchCriteria;

namespace Gibe.Umbraco.Blog.Filters
{
	public class SearchTermBlogPostFilter : IBlogPostFilter
	{
		private readonly string _searchTerm;

		public SearchTermBlogPostFilter(string searchTerm)
		{
			_searchTerm = searchTerm;
		}

		public IBooleanOperation GetCriteria(IQuery query)
		{
			return query.Field("bodyText", _searchTerm);
		}
	}
}
