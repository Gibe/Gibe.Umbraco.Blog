#if NET6
using Umbraco.Cms.Core.Models.PublishedContent;
#elif NET472
using Umbraco.Core.Models.PublishedContent;
#endif

namespace Gibe.Umbraco.Blog.Repositories
{
	public interface IBlogContentRepository
	{
		IPublishedContent BlogContent(int id);
	}
}
