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

		public string IndexName => _settingsService.GetSetting("Gibe.Umbraco.Blog.IndexName", "ExternalIndex");

		public string BlogPostDocumentTypeAlias => _settingsService.GetSetting("Gibe.Umbraco.Blog.BlogPostDocumentTypeAlias", "blogPost");
	}
}
