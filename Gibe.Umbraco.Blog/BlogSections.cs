using System.Collections.Generic;
using System.Linq;
using Examine;
using Gibe.Umbraco.Blog.Wrappers;
using Umbraco.Extensions;

namespace Gibe.Umbraco.Blog
{
	public class BlogSections<T> : IBlogSections<T> where T : class
	{
		private const string BlogSectionDocType = ""; // TODO
		private readonly ISearchIndex _searchIndex;

		public BlogSections(ISearchIndex searchIndex)
		{
			_searchIndex = searchIndex;
		}

		public IEnumerable<T> All()
		{
			var results = SearchForBlogSections();
			return Enumerable.Empty<T>();
			//return results.Select(r => Activator.Activate<T>(_umbracoWrapper.TypedContent(r.Id)));
		}
		
		private ISearchResults SearchForBlogSections()
		{
			var query = _searchIndex.CreateSearchQuery()
				.NodeTypeAlias(BlogSectionDocType);

			return query.Execute();
		}

	}
}
