#if NET5
using Umbraco.Cms.Core.Models.Membership;
#elif NET472
using Umbraco.Core.Models.Membership;
#endif

namespace Gibe.Umbraco.Blog.Models
{
	public class BlogAuthor
	{
		public IUser User { get; set; }

		public string Url { get; set; }
	}
}
