using Gibe.Umbraco.Blog.Models;

public class HardCodedBlogSettings : IBlogSettings
{
	public string IndexName => "ExternalIndex";

	public string BlogPostDocumentTypeAlias => "blogPost";
	public string BlogSectionDocumentTypeAlias => "blogSection";

	public string UserPickerPropertyEditorAlias => "Umbraco.UserPicker";

	public string UserPickerName => "User Picker - All Users";
}
