using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gibe.Umbraco.Blog.Models;
using Umbraco.Core.Models.Membership;

namespace Gibe.Umbraco.Blog
{
	public interface IBlogAuthors
	{
		IEnumerable<BlogAuthor> All(string rootPath);
	}

	public class FakeBlogAuthors : IBlogAuthors
	{
		private readonly IEnumerable<BlogAuthor> _authors;

		public FakeBlogAuthors(IEnumerable<BlogAuthor> authors)
		{
			_authors = authors;
		}

		public IEnumerable<BlogAuthor> All(string rootPath)
		{
			return _authors;
		}
	}
}
