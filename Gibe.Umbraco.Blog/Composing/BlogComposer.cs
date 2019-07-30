using Umbraco.Core;
using Umbraco.Core.Composing;
using Gibe.Umbraco.Blog.Wrappers;
using Gibe.Umbraco.Blog.Models;

namespace Gibe.Umbraco.Blog.Composing
{
	public class BlogComposer : IUserComposer
	{
		public void Compose(Composition composition)
		{
			composition.Components().Append<IndexEvents>();

			composition.RegisterUnique<IBlogArchive, BlogArchive>();
			composition.RegisterUnique<IBlogAuthors, BlogAuthors>();
			composition.RegisterUnique<IBlogSearch, BlogSearch>();
			composition.RegisterUnique<IBlogSections<BlogSectionsBase>, BlogSections<BlogSectionsBase>>();
			composition.RegisterUnique<IBlogService<BlogPostBase>, BlogService<BlogPostBase>>();
			composition.RegisterUnique<IBlogTags, BlogTags>();
			composition.RegisterUnique<ISearchIndex, NewsIndex>();
		}
	}
}
