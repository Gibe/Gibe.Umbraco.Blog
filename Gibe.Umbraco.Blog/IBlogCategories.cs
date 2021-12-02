using Gibe.Umbraco.Blog.Models;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Gibe.Umbraco.Blog
{
	public interface IBlogCategories
	{
		IEnumerable<BlogCategory> All(IPublishedContent blogRoot);
	}

	public class FakeBlogCategories : IBlogCategories
	{
		private readonly IEnumerable<BlogCategory> _blogCategories;

		public FakeBlogCategories(IEnumerable<BlogCategory> blogCategories)
		{
			_blogCategories = blogCategories;
		}

		public IEnumerable<BlogCategory> All(IPublishedContent blogRoot)
		{
			return _blogCategories;
		}
	}
}
