using System;
using System.Collections.Generic;
using Gibe.Pager.Models;
using Gibe.Umbraco.Blog.Filters;
using Gibe.Umbraco.Blog.Models;
using Gibe.Umbraco.Blog.Sort;

namespace Gibe.Umbraco.Blog
{
	public interface IBlogService<T> where T : IBlogPostModel
	{
		PageQueryResultModel<T> GetPosts(int itemsPerPage, int currentPage);
		PageQueryResultModel<T> GetPosts(int itemsPerPage, int currentPage, ISort sort);

		PageQueryResultModel<T> GetPosts(IEnumerable<IBlogPostFilter> filters, int itemsPerPage, int currentPage);
		PageQueryResultModel<T> GetPosts(IEnumerable<IBlogPostFilter> filters, int itemsPerPage, int currentPage, ISort sort);

		T GetNextPost(T blogPost, IEnumerable<IBlogPostFilter> filters, ISort sort);

		T GetPreviousPost(T blogPost, IEnumerable<IBlogPostFilter> filters, ISort sort);

		IEnumerable<T> GetRelatedPosts(T blogPost, int count);

		IEnumerable<T> GetRelatedPosts(IEnumerable<string> tags, int id, int count);
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

		public PageQueryResultModel<T> GetPosts(int itemsPerPage, int currentPage, ISort sort)
		{
			return GetPosts(itemsPerPage, currentPage);
		}

		public PageQueryResultModel<T> GetPosts(IEnumerable<IBlogPostFilter> filters, int itemsPerPage, int currentPage)
		{
			return GetPosts(itemsPerPage, currentPage);
		}

		public PageQueryResultModel<T> GetPosts(IEnumerable<IBlogPostFilter> filters, int itemsPerPage, int currentPage, ISort sort)
		{
			return GetPosts(filters, itemsPerPage, currentPage);
		}

		public T GetNextPost(T blogPost, IEnumerable<IBlogPostFilter> filters, ISort sort)
		{
			throw new NotImplementedException();
		}

		public T GetPreviousPost(T blogPost, IEnumerable<IBlogPostFilter> filters, ISort sort)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<T> GetRelatedPosts(T blogPost, int count)
		{
			return _results;
		}

		public IEnumerable<T> GetRelatedPosts(IEnumerable<string> tags, int id, int count)
		{
			return _results;
		}
	}
}