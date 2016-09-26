using System.Net;
using System.Net.Http;
using System.Web.Http;
using Gibe.Umbraco.Blog.Utilities;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Gibe.Umbraco.Blog.Services
{
  [PluginController("GibeUmbracoBlog")]
  public class BlogImportApiController : UmbracoApiController
  {
    [global::Umbraco.Web.WebApi.UmbracoAuthorize]
    [HttpPost]
    [ActionName("ImportAtomFeed")]
    public HttpResponseMessage ImportAtomFeed(ImportInfo info)
    {
      var import = new Import();
      import.ImportBlogPosts(info.FeedUrl, info.MediaFolderId, info.BlogRootId);
      return new HttpResponseMessage(HttpStatusCode.OK);
    }
  }

  public class ImportInfo
  {
    public string FeedUrl { get; set; }
    public int MediaFolderId { get; set; }
    public int BlogRootId { get; set; }
  }
}
