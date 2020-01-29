using System;
using System.Collections.Generic;

namespace Gibe.Umbraco.Blog.Models
{
	public interface IBlogPostModel
	{
		int Id { get; set; }
		string Url { get; set; }
		DateTime PostDate { get; set; }
		IEnumerable<string> Tags { get; set; }
		bool HasTags { get; }
	}
}
