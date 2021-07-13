using Gibe.Umbraco.Blog.Models;
#if NET5_0
using Umbraco.Cms.Core.Models.PublishedContent;
#elif NET472
using Umbraco.Core.Models.PublishedContent;
#endif

namespace Gibe.Umbraco.Blog
{
	public interface IBlogArchive
	{
		BlogArchiveModel All(IPublishedContent blogRoot);
	}

	public class FakeBlogArchive :IBlogArchive
	{
		private readonly BlogArchiveModel _blogArchiveModel;

		public FakeBlogArchive(BlogArchiveModel blogArchiveModel)
		{
			_blogArchiveModel = blogArchiveModel;
		}

		public BlogArchiveModel All(IPublishedContent blogRoot)
		{
			return _blogArchiveModel;
		}
	}
}
