using Gibe.Umbraco.Blog.Models;
using Umbraco.Core.Models.PublishedContent;

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
