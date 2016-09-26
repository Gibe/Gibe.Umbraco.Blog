using System;
using System.Collections.Generic;
using System.Linq;
using Gibe.Umbraco.Blog.Filters;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.Services;

namespace Gibe.Umbraco.Blog
{
	public class BlogAuthors : IBlogAuthors
	{
		private readonly IBlogSearch _blogSearch;
		private readonly IUserService _userService;

		public BlogAuthors(IBlogSearch blogSearch, IUserService userService)
		{
			_blogSearch = blogSearch;
			_userService = userService;
		}

		public IEnumerable<IUser> All()
		{
			var posts = _blogSearch.Search(Enumerable.Empty<IBlogPostFilter>());
			var allUserIds = posts.Select(p => Convert.ToInt32(p.Fields["postAuthor"])).Distinct();
			return allUserIds.Select(id => _userService.GetUserById(id));
		}
	}
}
