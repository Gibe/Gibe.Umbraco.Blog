using Gibe.Umbraco.Blog.Models;

#if NET472
using Gibe.Settings.Interfaces;

namespace Gibe.Umbraco.Blog.Models
{
	public class HardCodedBlogSettings : IBlogSettings
	{
		private readonly ISettingsService _settingsService;

		public HardCodedBlogSettings(ISettingsService settingsService)
		{
			_settingsService = settingsService;
		}

		public string IndexName => "ExternalIndex";

		public string BlogPostDocumentTypeAlias => _settingsService.GetSetting("Gibe.Umbraco.Blog.BlogPostDocumentTypeAlias", "blogPost");

		public string UserPickerPropertyEditorAlias => _settingsService.GetSetting("Gibe.Umbraco.Blog.UserPickerPropertyEditorAlias", "Umbraco.UserPicker");

		public string UserPickerName => _settingsService.GetSetting("Gibe.Umbraco.Blog.UserPickerName", "User Picker - All Users");
	}
}
#elif NET5_0
public class HardCodedBlogSettings : IBlogSettings
{
	public string IndexName => "ExternalIndex";

	public string BlogPostDocumentTypeAlias => "blogPost";

	public string UserPickerPropertyEditorAlias => "Umbraco.UserPicker";

	public string UserPickerName => "User Picker - All Users";
}
#endif
