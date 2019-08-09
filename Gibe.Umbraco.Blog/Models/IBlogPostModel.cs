using System;
using System.Collections.Generic;
using Umbraco.Core.Models.PublishedContent;

namespace Gibe.Umbraco.Blog.Models
{
	public interface IBlogPostModel
	{
		int Id { get; }
		string Url { get; }
		DateTime PostDate { get; }
		IEnumerable<string> Tags { get; }
		IPublishedContent Category { get; }
		bool HasTags { get; }
	}
}
