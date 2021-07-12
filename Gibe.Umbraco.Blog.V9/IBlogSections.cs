using System.Collections.Generic;
using Gibe.Umbraco.Blog.Models;

namespace Gibe.Umbraco.Blog
{
	public interface IBlogSections<out T> where T : class
	{
		IEnumerable<T> All();
	}

	public class FakeBlogSections<T> : IBlogSections<T> where T : class, IBlogPostSection
	{
		private readonly IEnumerable<T> _sections;

		public FakeBlogSections(IEnumerable<T> sections)
		{
			_sections = sections;
		}

		public IEnumerable<T> All()
		{
			return _sections;
		}
	}
}
