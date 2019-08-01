using System;
using System.Collections.Generic;
using System.Linq;
using Examine;
using Gibe.Pager.Interfaces;
using Gibe.Umbraco.Blog.Filters;
using Gibe.Umbraco.Blog.Models;
using NUnit.Framework;
using Moq;
using Umbraco.Core.Models.PublishedContent;
using Gibe.Umbraco.Blog.Repositories;

namespace Gibe.Umbraco.Blog.Tests
{
	[TestFixture]
	public class BlogServiceTests
	{
		private Mock<IPagerService> _pagerService;
		private Mock<IBlogContentRepository> _blogContentRepository;

		[SetUp]
		public void SetUp()
		{
			_pagerService = new Mock<IPagerService>();
			_blogContentRepository = new Mock<IBlogContentRepository>();

			_blogContentRepository.Setup(r => r.BlogContent(It.IsAny<int>()))
				.Returns((int id) => Content(id, "blogPost"));
		}

		[Test]
		public void GetRelatedPosts_Uses_Correct_Filters()
		{
			var blogSearch = new FakeBlogSearch(GetSearchResults());
			var blogService = new BlogService<BlogModel>(_pagerService.Object, blogSearch, _blogContentRepository.Object);
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

			Assert.That(blogSearch.LastUsedFilters.Any(t => typeof(AtLeastOneMatchingTagFilter) == t.GetType()), Is.EqualTo(true));

			var tagsFilter = blogSearch.LastUsedFilters.First(t => typeof(AtLeastOneMatchingTagFilter) == t.GetType()) as AtLeastOneMatchingTagFilter;
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
				SearchResult("1"),
				SearchResult("2"),
				SearchResult("3")
			});
		}

		private SearchResult SearchResult(string id)
		{
			var fields = new Dictionary<string, List<string>>();

			return new SearchResult(id, 1.0f, () => fields);
		}

		public static IPublishedContent Content(int id, string docType = "blogPost", string name = null)
		{
			var content = new Mock<IPublishedContent>();

			content.Setup(c => c.ContentType.Alias).Returns(docType);
			content.Setup(c => c.Name).Returns(name ?? docType);
			content.Setup(c => c.Id).Returns(id);

			return content.Object;
		}
	}

	public class BlogModel : IBlogPostModel
	{
		public int Id { get; set; }
		public string Url { get; }
		public DateTime PostDate { get; }
		public IEnumerable<string> Tags { get; set; }
		public bool HasTags { get; set; }
	}
}
