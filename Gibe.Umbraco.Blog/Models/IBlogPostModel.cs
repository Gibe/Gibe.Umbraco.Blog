using System;
using System.Collections.Generic;
#if NET6
using Umbraco.Cms.Core.Models.PublishedContent;
#elif NET472
using Umbraco.Core.Models.PublishedContent;
#endif

namespace Gibe.Umbraco.Blog.Models
{
	public interface IBlogPostModel
	{
		int Id { get; }
		DateTime PostDate { get; }
		IEnumerable<string> Tags { get; }
		IPublishedContent Category { get; }
		bool HasTags { get; }
	}
}
