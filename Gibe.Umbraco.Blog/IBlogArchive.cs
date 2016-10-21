using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gibe.Umbraco.Blog.Models;

namespace Gibe.Umbraco.Blog
{
	public interface IBlogArchive
	{
		BlogArchiveModel All(string rootPath);
	}

	public class FakeBlogArchive :IBlogArchive
	{
		private readonly BlogArchiveModel _blogArchiveModel;

		public FakeBlogArchive(BlogArchiveModel blogArchiveModel)
		{
			_blogArchiveModel = blogArchiveModel;
		}

		public BlogArchiveModel All(string rootPath)
		{
			return _blogArchiveModel;
		}
	}
}
