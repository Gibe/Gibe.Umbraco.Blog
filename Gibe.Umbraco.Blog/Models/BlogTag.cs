using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gibe.Umbraco.Blog.Models
{
	public class BlogTag
	{
		public string Tag { get; set; }
		public int Count { get; set; }
		public string Url { get; set; }
	}
}
