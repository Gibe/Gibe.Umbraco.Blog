namespace Gibe.Umbraco.Blog.Models
{
	public class BlogSettings : IBlogSettings
	{
		public string IndexName { get; set; } = "ExternalIndex";

		public string BlogPostDocumentTypeAlias { get; set; } = "blogPost";
		public string BlogSectionDocumentTypeAlias { get; set; } = "blogSection";

		public string UserPickerPropertyEditorAlias { get; set; } = "Umbraco.UserPicker";

		public string UserPickerName { get; set; } = "User Picker - All Users";
	}
}
