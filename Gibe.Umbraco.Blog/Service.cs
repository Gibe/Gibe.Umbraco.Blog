﻿#if NET5_0
using Examine.Lucene;
using Gibe.Pager.Extensions;
using Gibe.Pager.Interfaces;
using Gibe.Pager.Services;
using Gibe.Umbraco.Blog.Composing;
using Gibe.Umbraco.Blog.Models;
using Gibe.Umbraco.Blog.Repositories;
using Gibe.Umbraco.Blog.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;

namespace Gibe.Umbraco.Blog
{
	public static class NetCoreServicesExtensions
	{
		public static void AddGibeBlog<T>(this IServiceCollection services) where T : class, IBlogPostModel
		{
			services.AddGibePager<CurrentUrlService>();
			services.AddTransient<IPagerService, PagerService>();
			services.AddTransient<IBlogSettings, HardCodedBlogSettings>();
			services.AddTransient<IBlogService<T>, BlogService<T>>();

			services.AddUnique<IBlogContentRepository, BlogContentRepository>();
			services.AddUnique<IBlogArchive, BlogArchive>();
			services.AddUnique<IBlogAuthors, BlogAuthors>();
			services.AddUnique<IBlogSearch, BlogSearch>();
			services.AddUnique<IBlogTags, BlogTags>();
			services.AddUnique<IBlogCategories, BlogCategories>();
			services.AddUnique<ISearchIndex, NewsIndex>();

			//services.AddUnique<IConfigureOptions<LuceneDirectoryIndexOptions>, AddFieldsToExternalIndex>();
		}

		public class uSyncCoreComposer : IComposer
		{
			public void Compose(IUmbracoBuilder builder)
			{
				
			}
		}

		public class CurrentUrlService : ICurrentUrlService
		{
			private readonly HttpContextAccessor _httpContextAccessor;

			public CurrentUrlService(HttpContextAccessor httpContextAccessor)
			{
				_httpContextAccessor = httpContextAccessor;
			}

			public string CurrentUrl()
			{
				return _httpContextAccessor.HttpContext?.Request.GetDisplayUrl();
			}
		}
	}
}
#endif