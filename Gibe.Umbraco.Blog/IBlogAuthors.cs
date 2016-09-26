using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.Membership;

namespace Gibe.Umbraco.Blog
{
	public interface IBlogAuthors
	{
		IEnumerable<IUser> All();
	}

	public class FakeBlogAuthors : IBlogAuthors
	{
		private readonly IEnumerable<IUser> _authors;

		public FakeBlogAuthors(IEnumerable<IUser> authors)
		{
			_authors = authors;
		}

		public IEnumerable<IUser> All()
		{
			return _authors;
		}
	}
}
