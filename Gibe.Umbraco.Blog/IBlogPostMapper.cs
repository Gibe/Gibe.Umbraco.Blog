using System.Collections.Generic;
using System.Linq;
using Examine;
using Gibe.DittoServices.ModelConverters;
using Gibe.Umbraco.Blog.Models;
using Gibe.UmbracoWrappers;
using Umbraco.Core.Models;

namespace Gibe.Umbraco.Blog
{
	public interface IBlogPostMapper<T> where T : IBlogPostModel
	{
		IEnumerable<T> ToBlogPosts(IEnumerable<SearchResult> searchResults);
		T ToBlogPost(SearchResult searchResult);
	}
}
