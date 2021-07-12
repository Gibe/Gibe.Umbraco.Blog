using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Gibe.Umbraco.Blog.Models
{
	public abstract class BlogPostBase : PublishedContentModel, IBlogPostModel
	{
		protected BlogPostBase(IPublishedContent content) 
			: base(content) { }

		public DateTime PostDate => this.Value<DateTime>("postDate");

		public IEnumerable<string> Tags => this.Value<IEnumerable<string>>("settingsNewsTags");

		public IPublishedContent Category => this.Value<IPublishedContent>("category");

		public bool HasTags => Tags != null && Tags.Any();
	}
}