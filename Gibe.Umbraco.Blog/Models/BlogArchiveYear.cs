using System.Collections.Generic;

namespace Gibe.Umbraco.Blog.Models
{
  public class BlogArchiveYear : BlogPostGroup
  {
    public int Year { get; set; }
    public List<BlogArchiveMonth> Months { get; set; }
  }
}
