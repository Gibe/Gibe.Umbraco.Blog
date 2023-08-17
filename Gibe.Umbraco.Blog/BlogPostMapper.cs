using Examine;
using Gibe.Umbraco.Blog.Models;
using Gibe.Umbraco.Blog.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Gibe.Umbraco.Blog
{
	public class BlogPostMapper<T> : IBlogPostMapper<T> where T : class, IBlogPostModel
	{
		private readonly IBlogContentRepository _blogContentRepository;

		public BlogPostMapper(IBlogContentRepository blogContentRepository)
		{
			_blogContentRepository = blogContentRepository;
		}

		public T ToBlogPost(IPublishedContent content, IPublishedValueFallback fallback)
		{
			return Activator.Activate<T>(content, fallback);
		}

		public IEnumerable<T> ToBlogPosts(IEnumerable<IPublishedContent> content, IPublishedValueFallback fallback)
		{
			return content.Select(b => ToBlogPost(b, fallback));
		}

		public IEnumerable<T> ToBlogPosts(IEnumerable<ISearchResult> searchResults, IPublishedValueFallback fallback)
		{
			return ToBlogPosts(searchResults.Select(r => GetContent(r.Id)), fallback);
		}

		public IPublishedContent GetContent(string id)
		{
			var integerId = Convert.ToInt32(id);

			return _blogContentRepository.BlogContent(integerId);
		}
	}

}
