using System;
using System.Collections.Generic;
using System.Linq;
using Gibe.DittoProcessors.Processors;
using Our.Umbraco.Ditto;
using Umbraco.Core.Models.Membership;

namespace Gibe.Umbraco.Blog.Models
{
	public abstract class BlogPostBase : IBlogPostModel
	{
		public int Id { get; set; }
		public string Url { get; set; }
		public DateTime PostDate { get; set; }
		[UmbracoProperty("settingsNewsTags")]
		[Tags]
		public IEnumerable<string> Tags { get; set; }
		public bool HasTags => Tags != null && Tags.Any();
		[UmbracoProperty("postAuthor")]
		[UserPicker]
		public IUser Author { get; set; }
		public bool HasAuthor => Author != null;
	}
}