using Gibe.Settings.Interfaces;

namespace Gibe.Umbraco.Blog.Models
{
	public class BlogConfigSettings : IBlogSettings
	{
		private readonly ISettingsService _settingsService;

		public BlogConfigSettings(ISettingsService settingsService)
		{
			_settingsService = settingsService;
		}

		public string IndexName => "ExternalIndex";

		public string BlogPostDocumentTypeAlias => _settingsService.GetSetting("Gibe.Umbraco.Blog.BlogPostDocumentTypeAlias", "blogPost");

		public string UserPickerPropertyEditorAlias => _settingsService.GetSetting("Gibe.Umbraco.Blog.UserPickerPropertyEditorAlias", "Umbraco.UserPicker");

		public string UserPickerName => _settingsService.GetSetting("Gibe.Umbraco.Blog.UserPickerName", "User Picker - All Users");
	}
}
