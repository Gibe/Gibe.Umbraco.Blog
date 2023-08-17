using Examine;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Gibe.Umbraco.Blog
{
	public interface IBlogPostMapper<T>
	{
		IPublishedContent GetContent(string id);
		T ToBlogPost(IPublishedContent content, IPublishedValueFallback fallback);
		IEnumerable<T> ToBlogPosts(IEnumerable<IPublishedContent> content, IPublishedValueFallback fallback);
		IEnumerable<T> ToBlogPosts(IEnumerable<ISearchResult> searchResults, IPublishedValueFallback fallback);
	}
}