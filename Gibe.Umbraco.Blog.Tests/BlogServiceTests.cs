using System;
using System.Collections.Generic;
using Gibe.Umbraco.Blog.Models;
using NUnit.Framework;
using Umbraco.Core.Models.Membership;

namespace Gibe.Umbraco.Blog.Tests
{
	[TestFixture]
	public class BlogServiceTests
	{
		[Test]
		public void GetPosts_returns_all_posts_in_a_page()
		{

			//var blogService = new BlogService<BlogModel>(new FakeModelConverter(new List<BlogModel>()), new PagerService() , new FakeBlogSearch(), new DefaultUmbracoWrapper());
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
}
