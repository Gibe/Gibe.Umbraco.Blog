using System;
using System.Collections.Generic;
using Gibe.Umbraco.Blog.Models;
#if NET5_0
using Umbraco.Cms.Core.Models.PublishedContent;
#elif NET472
using Umbraco.Core.Models.PublishedContent;
#endif

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
