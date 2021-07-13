using System;
using System.Collections.Generic;
using System.Linq;
using Gibe.Umbraco.Blog.Filters;
using Gibe.Umbraco.Blog.Models;
using Gibe.Umbraco.Blog.Sort;
#if NET5_0
using Umbraco.Cms.Core.Services;
#elif NET472
using Umbraco.Core.Services;
#endif

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

		public IEnumerable<BlogAuthor> All(string rootPath)
		{
			var posts = _blogSearch.Search(Enumerable.Empty<IBlogPostFilter>(), new DateSort());
			var allUserIds = posts.Select(p => Convert.ToInt32(p.Values[ExamineFields.PostAuthor])).Distinct();

			return allUserIds.Select(id => _userService.GetUserById(id))
				.Select(user => new BlogAuthor {User = user, Url = $"{rootPath}?author={user.Name}"});
		}
	}
}
