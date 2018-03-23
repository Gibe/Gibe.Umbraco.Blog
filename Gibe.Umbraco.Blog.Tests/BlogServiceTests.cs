using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Examine;
using Gibe.DittoServices;
using Gibe.DittoServices.ModelConverters;
using Gibe.Pager.Interfaces;
using Gibe.Pager.Models;
using Gibe.Pager.Services;
using Gibe.Umbraco.Blog.Filters;
using Gibe.Umbraco.Blog.Models;
using Gibe.UmbracoWrappers;
using NUnit.Framework;
using Moq;
using Our.Umbraco.Ditto;
using umbraco.cms.presentation.create.controls;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Membership;
using Umbraco.Web.Models.TemplateQuery;

namespace Gibe.Umbraco.Blog.Tests
{
	[TestFixture]
	public class BlogServiceTests
	{
		private Mock<IPagerService> _pagerService;

		[SetUp]
		public void SetUp()
		{
			_pagerService = new Mock<IPagerService>();
		}

		[Test]
		public void GetPosts_returns_all_posts_in_a_page()
		{

			//var blogService = new BlogService<BlogModel>(new FakeModelConverter(new List<BlogModel>()), new PagerService() , new FakeBlogSearch(), new DefaultUmbracoWrapper());
		}

		[Test]
		public void GetRelatedPosts_Uses_Correct_Filters()
		{
			var blogSearch = new FakeBlogSearch(GetSearchResults());
			var blogService = new BlogService<BlogModel>(new FakeModelConverter(GetBlogPosts()), _pagerService.Object, blogSearch, UmbracoWrapper(Content(1), Content(2), Content(3)));
			var testPost = new BlogModel
			{
				Tags = new List<string>
				{
					"test",
					"post",
					"1"
				}
			};
			var relatedPosts = blogService.GetRelatedPosts(testPost, 3);

			Assert.That(blogSearch.LastUsedFilters.Any(t => typeof(TagsBlogPostFilter) == t.GetType()), Is.EqualTo(true));

			var tagsFilter = blogSearch.LastUsedFilters.First(t => typeof(TagsBlogPostFilter) == t.GetType()) as TagsBlogPostFilter;
			foreach (var tag in testPost.Tags)
			{
				Assert.That(tagsFilter.Tags.Contains(tag), Is.True);
			}

			Assert.That(relatedPosts.Count(), Is.EqualTo(3));
		}

		private FakeSearchResults GetSearchResults()
		{
			return new FakeSearchResults(new List<SearchResult>
			{
				SearchResult(1),
				SearchResult(2),
				SearchResult(3)
			});
		}

		private SearchResult SearchResult(int id)
		{
			return new SearchResult
			{
				Id = id
			};
		}

		private IEnumerable<BlogModel> GetBlogPosts()
		{
			return new List<BlogModel>
			{
				CreatePost(1, new []
				{
					"test"
				}),
				CreatePost(2, new []
				{
					"test",
					"post"
				}),
				CreatePost(3, new []
				{
					"Test"
				})
			};
		}

		private BlogModel CreatePost(int id, string[] tags)
		{
			return new BlogModel
			{
				Id = id,
				HasTags = true,
				Tags = tags
			};
		}

		public static IPublishedContent Content(int id, string docType = "blogPost", string name = null)
		{
			var content = new Mock<IPublishedContent>();
			content.Setup(c => c.DocumentTypeAlias).Returns(docType);
			content.Setup(c => c.Name).Returns(name ?? docType);
			content.Setup(c => c.Id).Returns(id);

			return content.Object;
		}


		public static IUmbracoWrapper UmbracoWrapper(params IPublishedContent[] content)
		{
			var umbraco = new Mock<IUmbracoWrapper>();
			umbraco.Setup(u => u.TypedContentAtRoot()).Returns(content);

			foreach (var c in content)
			{
				umbraco.Setup(u => u.TypedContent(c.Id)).Returns(c);
			}

			return umbraco.Object;
		}
	}

	public class BlogModel : IBlogPostModel
	{
		public int Id { get; set; }
		public string Url { get; set; }
		public DateTime PostDate { get; set; }
		public IEnumerable<string> Tags { get; set; }
		public bool HasTags { get; set; }
		public IUser Author { get; set; }
		public bool HasAuthor { get; set; }
	}

	public class FakeModelConverter : IModelConverter
	{
		private readonly IEnumerable<BlogModel> _blogPosts;

		public FakeModelConverter(IEnumerable<BlogModel> blogPosts)
		{
			_blogPosts = blogPosts;
		}

		public T ToModel<T>(IPublishedContent content, IEnumerable<DittoProcessorContext> contexts) where T : class
		{
			return _blogPosts.First(p => p.Id == content.Id) as T;
		}

		public object ToModel(Type type, IPublishedContent content, IEnumerable<DittoProcessorContext> contexts)
		{
			return ToModel<BlogModel>(content, contexts);
		}

		public IEnumerable<T> ToModel<T>(IEnumerable<IPublishedContent> nodes, IEnumerable<DittoProcessorContext> contexts) where T : class
		{
			return _blogPosts.Where(p => nodes.Select(n => n.Id).Contains(p.Id)).Select(p => p as T);
		}

		public IEnumerable<object> ToModel(Type type, IEnumerable<IPublishedContent> content, IEnumerable<DittoProcessorContext> contexts)
		{
			return ToModel<BlogModel>(content, contexts);
		}
	}
}
