using Umbraco.Cms.Core.Models.Membership;

namespace Gibe.Umbraco.Blog.Models
{
	public class BlogAuthor
	{
		public IUser User { get; set; }

		public string Url { get; set; }
	}
}
