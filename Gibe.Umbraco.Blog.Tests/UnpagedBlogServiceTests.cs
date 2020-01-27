using System.Collections.Generic;
using System.Linq;
using Examine;
using Gibe.Umbraco.Blog;
using Gibe.Umbraco.Blog.Tests;
using Moq;
using NUnit.Framework;

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
			var mock = _repository.Create<IBlogPostMapper<BlogModel>>();

			var mappedSearchResults = new List<BlogModel>();
			for (var i = 0; i < searchResultsCount; i++)
			{
				mappedSearchResults.Add(new BlogModel{Id = i});
			}

			mock.Setup(m => m.ToBlogPosts(It.IsAny<IEnumerable<SearchResult>>())).Returns(mappedSearchResults);

			return mock.Object;
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
			return new SearchResult
			{
				Id = id
			};
		}

		private IUnpagedBlogService<BlogModel> Service(IBlogSearch blogSearch, IBlogPostMapper<BlogModel> blogPostMapper)
			=> new UnpagedBlogService<BlogModel>(blogSearch, blogPostMapper);

		[TestCase(1, 5)]
		[TestCase(2, 7)]
		[TestCase(1, 10)]
		[TestCase(4, 8)]
		[TestCase(2, 3)]
		public void GetPosts_Returns_Expected_Range_Of_BlogPosts_And_Expected_Total(int startPost, int endPost)
		{
			var totalBlogPostsCount = 10;
			var expectedResultsCcount = endPost - startPost + 1;
			var searchResults = SearchResults(totalBlogPostsCount);
			
			var blogSearch = BlogSearch(searchResults);

			var service = Service(blogSearch, BlogPostMapper(expectedResultsCcount));

			var results = service.GetPosts(startPost, endPost);

			Assert.That(results.BlogPosts.Count(), Is.EqualTo(expectedResultsCcount));
			Assert.That(results.TotalItemsCount, Is.EqualTo(totalBlogPostsCount));
		}
	}
}
