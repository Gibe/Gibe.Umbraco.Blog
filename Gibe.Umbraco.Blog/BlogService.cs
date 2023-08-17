using Gibe.Pager.Interfaces;
using Gibe.Pager.Models;
using Gibe.Umbraco.Blog.Filters;
using Gibe.Umbraco.Blog.Models;
using Gibe.Umbraco.Blog.Sort;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Gibe.Umbraco.Blog
{
	public class BlogService<T> : IBlogService<T> where T : class, IBlogPostModel
	{
		private readonly IPagerService _pagerService;
		private readonly IBlogSearch _blogSearch;
		private readonly IBlogPostMapper<T> _blogPostMapper;

		public BlogService(
			IPagerService pagerService, 
			IBlogSearch blogSearch, 
			IBlogPostMapper<T> blogPostMapper)
		{
			_pagerService = pagerService;
			_blogSearch = blogSearch;
			_blogPostMapper = blogPostMapper;
		}

		public PageQueryResultModel<T> GetPosts(int itemsPerPage, int currentPage)
		{
			return GetPosts(itemsPerPage, currentPage, new DateSort());
		}

		public PageQueryResultModel<T> GetPosts(int itemsPerPage, int currentPage, ISort sort)
		{
			return GetPosts(Enumerable.Empty<IBlogPostFilter>(), itemsPerPage, currentPage, sort);
		}

		public PageQueryResultModel<T> GetPosts(IEnumerable<IBlogPostFilter> filters, int itemsPerPage, int currentPage)
		{
			return GetPosts(filters, itemsPerPage, currentPage, new DateSort());
		}

		public PageQueryResultModel<T> GetPosts(IEnumerable<IBlogPostFilter> filters, int itemsPerPage, int currentPage, ISort sort)
		{
			var posts = _blogSearch.Search(filters, sort, (currentPage - 1) * itemsPerPage, itemsPerPage);
			return PageQueryResultModel(itemsPerPage, currentPage, _blogPostMapper.ToBlogPosts(posts, new NoopPublishedValueFallback()), posts.TotalItemCount);
		}

		private PageQueryResultModel<T> PageQueryResultModel(int itemsPerPage, int currentPage, IEnumerable<T> posts, long totalPostCount)
		{
			return _pagerService.GetPageQueryResultModel(posts, itemsPerPage, currentPage, (int)totalPostCount);
		}

		public T GetNextPost(T current, IEnumerable<IBlogPostFilter> filters, ISort sort)
		{
			var results = _blogSearch.Search(filters, sort);
			var post = results.TakeWhile(r => r.Id != current.Id.ToString()).LastOrDefault();
			return post != null ? _blogPostMapper.ToBlogPost(_blogPostMapper.GetContent(post.Id), new NoopPublishedValueFallback()) : null;
		}

		public T GetPreviousPost(T current, IEnumerable<IBlogPostFilter> filters, ISort sort)
		{
			var results = _blogSearch.Search(filters, sort);
			var post = results.SkipWhile(r => r.Id != current.Id.ToString()).Skip(1).FirstOrDefault();
			return post != null ? _blogPostMapper.ToBlogPost(_blogPostMapper.GetContent(post.Id), new NoopPublishedValueFallback()) : null;
		}

		public IEnumerable<T> GetRelatedPosts(T post, int count)
		{
			return GetRelatedPosts(post.Tags, post.Id, count);
		}

		public IEnumerable<T> GetRelatedPosts(IEnumerable<string> tags, int postId, int count)
		{
			var filter = new AtLeastOneMatchingTagFilter(tags);
			var results = _blogSearch.Search(filter, new RelevanceSort()).Where(r => r.Id != postId.ToString());
			return _blogPostMapper.ToBlogPosts(results.Take(count), new NoopPublishedValueFallback());
		}
	}
}
