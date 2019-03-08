using System;
using System.Collections.Generic;

namespace Gibe.Umbraco.Blog.Models
{
	public interface IBlogPostModel
	{
		int Id { get; }
		string Url { get; }
		DateTime PostDate { get; }
		IEnumerable<string> Tags { get; }
		bool HasTags { get; }
	}
}
