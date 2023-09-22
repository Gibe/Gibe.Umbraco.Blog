using System.Collections.Generic;
using System.Linq;
using Examine;
using Gibe.Umbraco.Blog.Models;
using Gibe.Umbraco.Blog.Wrappers;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Gibe.Umbraco.Blog
{
	public class BlogSections<T> : IBlogSections<T> where T : class
	{
		private readonly ISearchIndex _searchIndex;
		private readonly IBlogSettings _blogSettings;
		private readonly IPublishedContentQuery _publishedContentQuery;
		private readonly IPublishedValueFallback _publishedValueFallback;

		public BlogSections(ISearchIndex searchIndex,
			 IBlogSettings blogSettings,
			 IPublishedContentQuery publishedContentQuery,
			 IPublishedValueFallback publishedValueFallback)
		{
			_searchIndex = searchIndex;
			_blogSettings = blogSettings;
			_publishedContentQuery = publishedContentQuery;
			_publishedValueFallback = publishedValueFallback;
		}

		public IEnumerable<T> All()
		{
			var results = SearchForBlogSections();
			return results.Select(r => Activator.Activate<T>(_publishedContentQuery.Content(r.Id), _publishedValueFallback));
		}
		
		private ISearchResults SearchForBlogSections()
		{
			var query = _searchIndex.CreateSearchQuery()
				.NodeTypeAlias(_blogSettings.BlogSectionDocumentTypeAlias);

			return query.Execute();
		}

	}
}
