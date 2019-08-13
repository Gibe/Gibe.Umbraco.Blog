using System;
using System.Collections.Generic;
using Gibe.Umbraco.Blog.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Gibe.Umbraco.Blog
{
	public interface IBlogTags
	{
		IEnumerable<BlogTag> All(IPublishedContent blogRoot);
	}

	public class FakeBlogTags : IBlogTags
	{
		private readonly IEnumerable<BlogTag> _blogTags;

		public FakeBlogTags(IEnumerable<BlogTag> blogTags)
		{
			_blogTags = blogTags;
		}

		public IEnumerable<BlogTag> All(IPublishedContent blogRoot)
		{
			return _blogTags;
		}
	}
}
