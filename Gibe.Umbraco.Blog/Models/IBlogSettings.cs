﻿namespace Gibe.Umbraco.Blog.Models
{
	public interface IBlogSettings
	{
		string IndexName { get; }

		string BlogSectionTypeAlias { get; }

		string BlogPostDocumentTypeAlias { get; }

		string UserPickerPropertyEditorAlias { get; }

		string UserPickerName { get; }
	}
}
