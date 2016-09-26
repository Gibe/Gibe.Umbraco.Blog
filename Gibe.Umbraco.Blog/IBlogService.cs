using System;
using System.Collections.Generic;
using Gibe.Pager.Models;
using Gibe.Umbraco.Blog.Filters;
using Gibe.Umbraco.Blog.Models;
using Umbraco.Core.Models;

namespace Gibe.Umbraco.Blog
{
	public interface IBlogService<T> where T : IBlogPostModel
	{
		PageQueryResultModel<T> GetPosts(int itemsPerPage, int currentPage);

		PageQueryResultModel<T> GetPosts(IEnumerable<IBlogPostFilter> filters, int itemsPerPage, int currentPage);

		T GetNextPost(T blogPost, IEnumerable<IBlogPostFilter> filters);

		T GetPreviousPost(T blogPost, IEnumerable<IBlogPostFilter> filters);

		IEnumerable<T> GetRelatedPosts(T blogPost, int count);
	}

	public class FakeBlogService<T> : IBlogService<T> where T : IBlogPostModel
	{
		private readonly IEnumerable<T> _results;

		public FakeBlogService(IEnumerable<T> results)
		{
			_results = results;
		}

		public PageQueryResultModel<T> GetPosts(int itemsPerPage, int currentPage)
		{
			return new PageQueryResultModel<T>
			{
				CurrentPage = currentPage,
				CurrentPageItems = _results,
				ItemsPerPage = itemsPerPage,
				TotalPages = 1
			};
		}

		public PageQueryResultModel<T> GetPosts(IEnumerable<IBlogPostFilter> filters, int itemsPerPage, int currentPage)
		{
			return GetPosts(itemsPerPage, currentPage);
		}

		public T GetNextPost(T blogPost, IEnumerable<IBlogPostFilter> filters)
		{
			throw new NotImplementedException();
		}

		public T GetPreviousPost(T blogPost, IEnumerable<IBlogPostFilter> filters)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<T> GetRelatedPosts(T blogPost, int count)
		{
			return _results;
		}
	}
}