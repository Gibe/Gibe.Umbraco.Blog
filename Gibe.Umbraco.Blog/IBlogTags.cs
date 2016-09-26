using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gibe.Umbraco.Blog.Models;
using Umbraco.Core.Models;

namespace Gibe.Umbraco.Blog
{
	public interface IBlogTags
	{
		IEnumerable<BlogTag> All();
	}

	public class FakeBlogTags : IBlogTags
	{
		private readonly IEnumerable<BlogTag> _blogTags;

		public FakeBlogTags(IEnumerable<BlogTag> blogTags)
		{
			_blogTags = blogTags;
		}

		public IEnumerable<BlogTag> All()
		{
			return _blogTags;
		}
	}
}
