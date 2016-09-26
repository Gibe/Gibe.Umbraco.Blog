using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gibe.Umbraco.Blog.Models;

namespace Gibe.Umbraco.Blog
{
	public interface IBlogSections<out T> where T : IBlogPostSection
	{
		IEnumerable<T> All();
	}

	public class FakeBlogSections<T> : IBlogSections<T> where T : IBlogPostSection
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
