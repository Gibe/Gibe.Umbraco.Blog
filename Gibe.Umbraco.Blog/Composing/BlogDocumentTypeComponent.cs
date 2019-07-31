using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Gibe.Umbraco.Blog.Composing
{
	public class BlogDocumentTypeComponent : IComponent
	{
		private readonly IContentTypeService _contentTypeService;

		public BlogDocumentTypeComponent(IContentTypeService contentTypeService)
		{
			_contentTypeService = contentTypeService;
		}

		public void Initialize()
		{

		}

		public void Terminate()
		{
			
		}
	}
}
