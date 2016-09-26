using System;
using System.Collections.Generic;
using System.Linq;
using Examine;
using Gibe.DittoServices.ModelConverters;
using Gibe.Pager.Interfaces;
using Gibe.Pager.Models;
using Gibe.Umbraco.Blog.Filters;
using Gibe.Umbraco.Blog.Models;
using Gibe.UmbracoWrappers;
using Umbraco.Core.Models;

namespace Gibe.Umbraco.Blog
{
	public class BlogService<T> : IBlogService<T> where T : class, IBlogPostModel
	{
		private readonly IModelConverter _modelConverter;
		private readonly IPagerService _pagerService;
		private readonly IUmbracoWrapper _umbracoWrapper;
		private readonly IBlogSearch _blogSearch;

		public BlogService(IModelConverter modelConverter, IPagerService pagerService, IBlogSearch blogSearch, IUmbracoWrapper umbracoWrapper)
		{
			_modelConverter = modelConverter;
			_pagerService = pagerService;
			_blogSearch = blogSearch;
			_umbracoWrapper = umbracoWrapper;
		}

		public PageQueryResultModel<T> GetPosts(int itemsPerPage, int currentPage)
		{
			return GetPosts(Enumerable.Empty<IBlogPostFilter>(), itemsPerPage, currentPage);
		}
		
		public PageQueryResultModel<T> GetPosts(IEnumerable<IBlogPostFilter> filters, int itemsPerPage, int currentPage)
		{
			var results = _blogSearch.Search(filters, true);
			var posts = results.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage);
			return PageQueryResultModel(itemsPerPage, currentPage, ToBlogPosts(posts), results.TotalItemCount);
		}
		
		private PageQueryResultModel<T> PageQueryResultModel(int itemsPerPage, int currentPage, IEnumerable<T> posts, int totalPostCount)
		{
			return _pagerService.GetPageQueryResultModel(posts, itemsPerPage, currentPage, totalPostCount);
		}
		
		private T ToBlogPost(IPublishedContent content)
		{
			return _modelConverter.ToModel<T>(content);
		}

		private IEnumerable<T> ToBlogPosts(IEnumerable<IPublishedContent> content)
		{
			return content.Select(ToBlogPost);
		}

		private IEnumerable<T> ToBlogPosts(IEnumerable<SearchResult> searchResults)
		{
			return ToBlogPosts(searchResults.Select(r => _umbracoWrapper.TypedContent(r.Id)));
		}

		public T GetNextPost(T current, IEnumerable<IBlogPostFilter> filters)
		{
			throw new NotImplementedException();
		}

		public T GetPreviousPost(T current, IEnumerable<IBlogPostFilter> filters)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<T> GetRelatedPosts(T post, int count)
		{
			throw new NotImplementedException();
		}
	}
}