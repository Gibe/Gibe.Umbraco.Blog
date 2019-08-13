using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Gibe.Umbraco.Blog.Repositories
{
	public class BlogContentRepository : IBlogContentRepository
	{
		private readonly IUmbracoContextFactory _umbracoContextFactory;

		public BlogContentRepository(IUmbracoContextFactory umbracoContextFactory)
		{
			_umbracoContextFactory = umbracoContextFactory;
		}

		public IPublishedContent BlogContent(int id)
		{
			using (var context = _umbracoContextFactory.EnsureUmbracoContext())
			{
				return context.UmbracoContext.Content.GetById(id);
			}
		}
	}
}
