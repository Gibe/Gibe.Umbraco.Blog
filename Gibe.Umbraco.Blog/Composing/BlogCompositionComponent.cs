using Gibe.Umbraco.Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Gibe.Umbraco.Blog.Composing
{
	public class BlogCompositionComponent : IComponent
	{
		private readonly IContentTypeService _contentTypeService;
		private readonly IDataTypeService _dataTypeService;
		private readonly IBlogSettings _blogSettings;
		private readonly PropertyEditorCollection _propertyEditors;

		private string _blogPostCompositionDocumentTypeAlias => $"{_blogSettings.BlogPostDocumentTypeAlias}Composition";

		public BlogCompositionComponent(IContentTypeService contentTypeService,
			IDataTypeService dataTypeService,
			IBlogSettings blogSettings,
			PropertyEditorCollection propertyEditors)
		{
			_contentTypeService = contentTypeService;
			_dataTypeService = dataTypeService;
			_blogSettings = blogSettings;
			_propertyEditors = propertyEditors;
		}

		public void Initialize()
		{
			var existingBlogComposition = _contentTypeService.Get(_blogPostCompositionDocumentTypeAlias);

			if (existingBlogComposition != null)
			{
				return;
			}

			_dataTypeService.Save(UserPicker());
			_contentTypeService.Save(BlogComposition());
		}

		private IContentType BlogComposition()
		{
			var blogComposition = new ContentType(-1)
			{
				Alias = _blogPostCompositionDocumentTypeAlias,
				Name = "Blog Post Composition"
			};

			blogComposition.AddPropertyGroup("Content");
			blogComposition.AddPropertyType(PostDate, "Content");
			blogComposition.AddPropertyType(PostAuthor, "Content");
			blogComposition.AddPropertyType(Tags, "Content");

			return blogComposition;
		}

		private DataType UserPicker()
		{
			var propertyEditorExists = _propertyEditors.TryGet(_blogSettings.UserPickerPropertyEditorAlias, out IDataEditor dataEditor);

			if (!propertyEditorExists)
			{
				throw new InvalidOperationException($"Unable to compose blog post composition as the '{_blogSettings.UserPickerPropertyEditorAlias}' property editor does not exist.");
			}

			var userPicker = new DataType(dataEditor, -1)
			{
				Name = _blogSettings.UserPickerName
			};
			
			return userPicker;
		}

		private PropertyType PostDate => 
			new PropertyType(_dataTypeService.GetDataType("Date picker with time"), "postDate") { Name = "Post Date" };

		private PropertyType PostAuthor => 
			new PropertyType(_dataTypeService.GetDataType(_blogSettings.UserPickerName), "postAuthor") { Name = "Post Author" };

		private PropertyType Tags =>  
			new PropertyType(_dataTypeService.GetDataType("Tags"), "settingsNewsTags") { Name = "Tags" };

		public void Terminate()
		{
			
		}
	}
}
