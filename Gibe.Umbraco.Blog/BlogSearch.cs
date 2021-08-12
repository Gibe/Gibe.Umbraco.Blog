using System.Collections.Generic;
using Examine;
using Examine.Search;
using Gibe.Umbraco.Blog.Filters;
using Gibe.Umbraco.Blog.Models;
using Gibe.Umbraco.Blog.Sort;
using Gibe.Umbraco.Blog.Wrappers;
using StackExchange.Profiling;
#if NET472
using Umbraco.Examine;
#elif NET5_0
using Umbraco.Extensions;
#endif

namespace Gibe.Umbraco.Blog
{
	public class BlogSearch : IBlogSearch
	{
		private readonly ISearchIndex _newsIndex;
		private readonly IBlogSettings _blogSettings;

		public BlogSearch(ISearchIndex newsIndex,
			IBlogSettings blogSettings)
		{
			_newsIndex = newsIndex;
			_blogSettings = blogSettings;
		}
		
		public ISearchResults Search(IBlogPostFilter filter, ISort sort)
		{
			return Search(new List<IBlogPostFilter> { filter }, sort);
		}
		
		public ISearchResults Search(IEnumerable<IBlogPostFilter> filters, ISort sort)
		{
			return SearchForBlogPosts(GetSearchQuery(filters, sort));
		}

		public ISearchResults Search(IBlogPostFilter filter, ISort sort, int skip, int take)
		{
			return Search(new List<IBlogPostFilter> { filter }, sort, skip, take);
		}

		public ISearchResults Search(IEnumerable<IBlogPostFilter> filters, ISort sort, int skip, int take)
		{
			return SearchForBlogPosts(GetSearchQuery(filters, sort), skip, take);
		}

		private IOrdering GetSearchQuery(IEnumerable<IBlogPostFilter> filters, ISort sort)
		{		
			return sort.GetCriteria(GetQuery(filters));
		}

		private IBooleanOperation GetQuery(IEnumerable<IBlogPostFilter> filters)
		{
			var query = _newsIndex.CreateSearchQuery().NodeTypeAlias(_blogSettings.BlogPostDocumentTypeAlias);
			foreach (var filter in filters)
			{
				query = filter.GetCriteria(query.And());
			}
			return query;
		}

		private ISearchResults SearchForBlogPosts(IOrdering operation, int? skip = null, int? take = null)
		{
			using (MiniProfiler.Current.Step("Search"))
			{
				if (skip.HasValue)
				{
					var options = new QueryOptions(skip.Value, take);
					return operation.Execute(options);
				}
				return operation.Execute();
			}
		}
	}
	
}
