using System;
using System.Collections.Generic;
using System.Linq;
#if NET6
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;
#elif NET472
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
#endif


namespace Gibe.Umbraco.Blog.Models
{
	public abstract class BlogPostBase : PublishedContentModel, IBlogPostModel
	{
#if NET6
		protected BlogPostBase(IPublishedContent content, IPublishedValueFallback publishedValueFallback)
			: base(content, publishedValueFallback) { }
#elif NET472
		protected BlogPostBase(IPublishedContent content) 
			: base(content) { }
#endif

		public DateTime PostDate => this.Value<DateTime>("postDate");

		public IEnumerable<string> Tags => this.Value<IEnumerable<string>>("settingsNewsTags");

		public IPublishedContent Category => this.Value<IPublishedContent>("category");

		public bool HasTags => Tags != null && Tags.Any();
	}
}