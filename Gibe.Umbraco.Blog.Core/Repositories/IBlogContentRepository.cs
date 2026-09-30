using Umbraco.Cms.Core.Models.PublishedContent;

namespace Gibe.Umbraco.Blog.Repositories
{
	public interface IBlogContentRepository
	{
		IPublishedContent BlogContent(int id);
	}
}
