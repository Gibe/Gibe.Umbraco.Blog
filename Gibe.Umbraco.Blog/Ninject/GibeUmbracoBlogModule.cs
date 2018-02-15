using Gibe.Umbraco.Blog.Models;
using Gibe.Umbraco.Blog.Wrappers;
using Ninject.Modules;

namespace Gibe.Umbraco.Blog.Ninject
{
	public class GibeUmbracoBlogModule<TBlogPostModel, TBlogSectionModel> : NinjectModule 
		where TBlogPostModel : class, IBlogPostModel
		where TBlogSectionModel : class, IBlogPostSection
	{
		public override void Load()
		{
			Bind<IBlogArchive>().To<BlogArchive>();
			Bind<IBlogAuthors>().To<BlogAuthors>();
			Bind<IBlogSearch>().To<BlogSearch>();
			Bind<IBlogSections<TBlogSectionModel>>().To<BlogSections<TBlogSectionModel>>();
			Bind<IBlogService<TBlogPostModel>>().To<BlogService<TBlogPostModel>>();
			Bind<IBlogTags>().To<BlogTags>();
			Bind<ISearchIndex>().To<NewsIndex>();
		}
	}
}
