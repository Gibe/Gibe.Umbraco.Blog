using System;
using System.Collections.Generic;
using System.Linq;
using Examine;
using Gibe.Umbraco.Blog.Models;
using Gibe.Umbraco.Blog.Wrappers;
using Umbraco.Examine;
using Umbraco.Web;

namespace Gibe.Umbraco.Blog
{
	public class BlogSections<T> : IBlogSections<T> where T : class
	{
		private readonly IUmbracoContextAccessor _umbracoContextAccessor;
		private readonly ISearchIndex _searchIndex;
		private readonly IBlogSettings _blogSettings;

		public BlogSections(IUmbracoContextAccessor umbracoContextAccessor, ISearchIndex searchIndex, IBlogSettings blogSettings)
		{
			_umbracoContextAccessor = umbracoContextAccessor;
			_searchIndex = searchIndex;
			_blogSettings = blogSettings;
		}

		public IEnumerable<T> All()
		{
			var results = SearchForBlogSections();
			return results.Select(r => Activator.Activate<T>(_umbracoContextAccessor.UmbracoContext.Content.GetById(Int32.Parse(r.Id))));
		}
		
		private ISearchResults SearchForBlogSections()
		{
			var query = _searchIndex.CreateSearchQuery()
				.NodeTypeAlias(_blogSettings.BlogSectionTypeAlias);

			return query.Execute();
		}

	}
}
