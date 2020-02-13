namespace Gibe.Umbraco.Blog.Settings
{
	public interface IBlogSettings
	{
		string BlogRootDoctype { get; }
		string BlogPostDoctype { get; }
		string BlogSectionDoctype { get;}
	}

	public class DefaultBlogSettings : IBlogSettings
	{
		public string BlogRootDoctype => "blog";
		public string BlogPostDoctype => "blogPost";
		public string BlogSectionDoctype => "blogSection";
	}
}
