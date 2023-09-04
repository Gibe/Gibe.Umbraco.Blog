using System.Collections.Generic;
using System.Linq;
using Examine;
using Gibe.Umbraco.Blog;
using Gibe.Umbraco.Blog.Tests;
using Moq;
using NUnit.Framework;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Gibe.UmbracoBlog.Tests
{
	[TestFixture]
	public class UnpagedBlogServiceTests
	{
		private MockRepository _repository;

		[SetUp]
		public void Setup()
		{
			_repository = new MockRepository(MockBehavior.Strict);
		}

		private IBlogPostMapper<BlogModel> BlogPostMapper(int searchResultsCount)
		{
			var blogPostMapper = _repository.Create<IBlogPostMapper<BlogModel>>();

			var mappedSearchResults = new List<BlogModel>();
			for (var i = 0; i < searchResultsCount; i++)
			{
				mappedSearchResults.Add(new BlogModel { Id = i });
			}

			blogPostMapper.Setup(m => m.ToBlogPosts(It.IsAny<IEnumerable<ISearchResult>>(), It.IsAny<NoopPublishedValueFallback>())).Returns(mappedSearchResults);

			return blogPostMapper.Object;
		}

		private IBlogSearch BlogSearch(ISearchResults searchResults)
		{
			return new FakeBlogSearch(searchResults);
		}

		private ISearchResults SearchResults(int searchResultsCount)
		{
			var searchResults = new List<SearchResult>();
			for (var i = 0; i < searchResultsCount; i++)
			{
				searchResults.Add(SearchResult(i));
			}
			return new FakeSearchResults(searchResults);
		}

		private SearchResult SearchResult(int id)
		{
			return new SearchResult(id.ToString(), 0, () => new Dictionary<string, List<string>>());
		}

		private IUnpagedBlogService<BlogModel> Service(IBlogSearch blogSearch, IBlogPostMapper<BlogModel> blogPostMapper)
			=> new UnpagedBlogService<BlogModel>(blogSearch, blogPostMapper);

		[TestCase(1, 5, false)]
		[TestCase(2, 7, false)]
		[TestCase(1, 10, true)]
		[TestCase(4, 8, true)]
		[TestCase(2, 3, false)]
		public void GetPosts_Returns_Expected_Range_Of_BlogPosts_And_Expected_Total(int startPost, int postCount, bool isLastPage)
		{
			var totalBlogPostsCount = 10;
			var searchResults = SearchResults(totalBlogPostsCount);

			var blogSearch = BlogSearch(searchResults);

			var service = Service(blogSearch, BlogPostMapper(postCount));

			var results = service.GetPosts(startPost, postCount);

			Assert.That(results.BlogPosts.Count(), Is.EqualTo(postCount));
			Assert.That(results.TotalItemsCount, Is.EqualTo(totalBlogPostsCount));
			Assert.That(results.IsLastPage, Is.EqualTo(isLastPage));
		}
	}
}
