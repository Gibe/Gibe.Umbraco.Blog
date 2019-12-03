using System.Collections.Generic;
using System.Linq;
using Examine;
using Gibe.DittoServices.ModelConverters;
using Gibe.Umbraco.Blog.Models;
using Gibe.Umbraco.Blog.Wrappers;
using Gibe.UmbracoWrappers;

namespace Gibe.Umbraco.Blog
{
	public class BlogSections<T> : IBlogSections<T> where T : class,IBlogPostSection
	{
		private const string BlogSectionDocType = "blogSection";
		private readonly IUmbracoWrapper _umbracoWrapper;
		private readonly IModelConverter _modelConverter;
		private readonly ISearchIndex _searchIndex;

		public BlogSections(IUmbracoWrapper umbracoWrapper, IModelConverter modelConverter, ISearchIndex searchIndex)
		{
			_umbracoWrapper = umbracoWrapper;
			_modelConverter = modelConverter;
			_searchIndex = searchIndex;
		}

		public IEnumerable<T> All()
		{
			var results = SearchForBlogSections();
			return results.Select(r => _modelConverter.ToModel<T>(_umbracoWrapper.TypedContent(r.Id)));
		}
		
		private ISearchResults SearchForBlogSections()
		{
			var query = _searchIndex.CreateSearchCriteria().NodeTypeAlias(BlogSectionDocType).Compile();
			
			return _searchIndex.Search(query);
		}

	}
}
