using Gibe.Pager.Models;
using Gibe.Umbraco.Blog;
using Gibe.Umbraco.Blog.DemoSite.Models;
using Gibe.Umbraco.Blog.Filters;
using Gibe.Umbraco.Blog.Models;
using Gibe.Umbraco.Blog.Repositories;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Gibe.Umbraco.Blog.DemoSite.Controllers
{
	// Plain MVC routes rather than Umbraco content templates - these pages exercise
	// IBlogService<T> directly and aren't backed by their own Umbraco doctype/template.
	public class DemoController : Controller
	{
		private const int HomePagePostCount = 3;
		private const int BlogPagePostsPerPage = 5;
		private const string BlogSectionDocumentTypeAlias = "blogSection";

		private readonly IBlogService<BlogPost> _blogService;
		private readonly IBlogTags _blogTags;
		private readonly IBlogCategories _blogCategories;
		private readonly IBlogContentRepository _blogContentRepository;
		private readonly IBlogPostMapper<BlogPost> _blogPostMapper;
		private readonly IPublishedContentQuery _publishedContentQuery;

		public DemoController(
			IBlogService<BlogPost> blogService,
			IBlogTags blogTags,
			IBlogCategories blogCategories,
			IBlogContentRepository blogContentRepository,
			IBlogPostMapper<BlogPost> blogPostMapper,
			IPublishedContentQuery publishedContentQuery)
		{
			_blogService = blogService;
			_blogTags = blogTags;
			_blogCategories = blogCategories;
			_blogContentRepository = blogContentRepository;
			_blogPostMapper = blogPostMapper;
			_publishedContentQuery = publishedContentQuery;
		}

		[HttpGet("/")]
		public IActionResult Home()
		{
			var recentPosts = _blogService.GetPosts(HomePagePostCount, 1).CurrentPageItems;
			return View("~/Views/Demo/Home.cshtml", recentPosts);
		}

		[HttpGet("/blog")]
		public IActionResult Blog(int page = 1, string? tag = null, string? category = null)
		{
			var filters = new List<IBlogPostFilter>();
			if (!string.IsNullOrWhiteSpace(tag))
			{
				filters.Add(new TagBlogPostFilter(tag));
			}
			if (!string.IsNullOrWhiteSpace(category))
			{
				filters.Add(new CategoryBlogPostFilter(category));
			}

			var blogRoot = GetBlogRoot();

			var model = new BlogListViewModel
			{
				Posts = _blogService.GetPosts(filters, BlogPagePostsPerPage, page < 1 ? 1 : page),
				Tags = blogRoot != null ? _blogTags.All(blogRoot) : Enumerable.Empty<BlogTag>(),
				Categories = blogRoot != null ? _blogCategories.All(blogRoot) : Enumerable.Empty<BlogCategory>(),
				SelectedTag = tag,
				SelectedCategory = category
			};

			return View("~/Views/Demo/Blog.cshtml", model);
		}

		[HttpGet("/blog/{id:int}")]
		public IActionResult Post(int id)
		{
			var content = _blogContentRepository.BlogContent(id);
			if (content == null)
			{
				return NotFound();
			}

			var post = _blogPostMapper.ToBlogPost(content, new NoopPublishedValueFallback());
			return View("~/Views/Demo/BlogPost.cshtml", post);
		}

		private IPublishedContent? GetBlogRoot()
		{
			return _publishedContentQuery.ContentAtRoot()
				.SelectMany(c => c.DescendantsOrSelf())
				.FirstOrDefault(c => c.ContentType.Alias == BlogSectionDocumentTypeAlias);
		}
	}

	public class BlogListViewModel
	{
		public PageQueryResultModel<BlogPost> Posts { get; set; } = null!;
		public IEnumerable<BlogTag> Tags { get; set; } = Enumerable.Empty<BlogTag>();
		public IEnumerable<BlogCategory> Categories { get; set; } = Enumerable.Empty<BlogCategory>();
		public string? SelectedTag { get; set; }
		public string? SelectedCategory { get; set; }
	}
}
