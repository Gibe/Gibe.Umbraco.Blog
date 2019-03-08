using System;
using System.Collections.Generic;
using System.Linq;
using Examine;
using Gibe.Umbraco.Blog.Models;
using Gibe.Umbraco.Blog.Wrappers;
using Gibe.UmbracoWrappers;

namespace Gibe.Umbraco.Blog
{
	public class BlogSections<T> : IBlogSections<T> where T : class
	{
		private const string BlogSectionDocType = ""; // TODO
		private readonly IUmbracoWrapper _umbracoWrapper;
		private readonly ISearchIndex _searchIndex;

		public BlogSections(IUmbracoWrapper umbracoWrapper, ISearchIndex searchIndex)
		{
			_umbracoWrapper = umbracoWrapper;
			_searchIndex = searchIndex;
		}

		public IEnumerable<T> All()
		{
			var results = SearchForBlogSections();
			return results.Select(r => Activator.Activate<T>(_umbracoWrapper.TypedContent(r.Id)));
		}
		
		private ISearchResults SearchForBlogSections()
		{
			var query = _searchIndex.CreateSearchCriteria().NodeTypeAlias(BlogSectionDocType).Compile();
			
			return _searchIndex.Search(query);
		}

	}
}
