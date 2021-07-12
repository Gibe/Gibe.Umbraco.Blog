using System.Collections.Generic;
using Gibe.Umbraco.Blog.Models;

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
