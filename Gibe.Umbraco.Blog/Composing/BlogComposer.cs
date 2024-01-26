using Umbraco.Core;
using Umbraco.Core.Composing;
using Gibe.Umbraco.Blog.Wrappers;
using Gibe.Umbraco.Blog.Models;
using Gibe.Settings.Interfaces;
using Gibe.Settings.Implementations;
using Gibe.Pager.Interfaces;
using Gibe.Pager.Services;
using Gibe.Umbraco.Blog.Repositories;

namespace Gibe.Umbraco.Blog.Composing
{
	public class BlogComposer : IUserComposer
	{
		public void Compose(Composition composition)
		{
			composition.Components().Append<BlogCompositionComponent>();
			composition.Components().Append<IndexEventsComponent>();

			composition.Register<IPagerService, PagerService>();
			composition.Register<ISettingsService, SettingsService>();
			composition.Register<IBlogSettings, BlogConfigSettings>();

			composition.Register<IBlogContentRepository, BlogContentRepository>();
			composition.Register<IBlogArchive, BlogArchive>();
			composition.Register<IBlogAuthors, BlogAuthors>();
			composition.Register<IBlogSearch, BlogSearch>();
			composition.Register<IBlogTags, BlogTags>();	
			composition.Register<IBlogCategories, BlogCategories>();
			composition.Register<ISearchIndex, NewsIndex>();
		}
	}
}
