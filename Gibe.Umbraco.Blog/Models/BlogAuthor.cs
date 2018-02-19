using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using Umbraco.Core.Models.Membership;

namespace Gibe.Umbraco.Blog.Models
{
	public class BlogAuthor
	{
		public IUser User { get; set; }

		public string Url { get; set; }
	}
}
