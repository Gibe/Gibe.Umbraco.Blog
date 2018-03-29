using System;
using System.Collections.Generic;
using Umbraco.Core.Models.Membership;

namespace Gibe.Umbraco.Blog.Models
{
	public interface IBlogPostModel
	{
		int Id { get; set; }
		string Url { get; set; }
		DateTime PostDate { get; set; }
		IEnumerable<string> Tags { get; set; }
		bool HasTags { get; }
		IUser Author { get; set; }
		bool HasAuthor { get; }
		IEnumerable<string> Categories { get; set; }
		bool HasCategories { get; }
	}
}
