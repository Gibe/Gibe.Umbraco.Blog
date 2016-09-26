using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gibe.Umbraco.Blog.Models
{
  public class BlogArchiveYear : BlogPostGroup
  {
    public int Year { get; set; }
    public List<BlogArchiveMonth> Months { get; set; }
  }
}
