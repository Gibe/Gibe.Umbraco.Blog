using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace Gibe.Umbraco.Blog.Utilities
{
  public class Import
  {
    /// <summary>
    /// Temp action to import blog posts from Blogger
    /// </summary>
    /// <param name="atomUrl">The URL of the atom feed to add from</param>
    /// <param name="mediaFolderId">The media folder to add any images to</param>
    /// <param name="blogRootId">The blog root to add itmes to</param>
    /// <returns></returns>
    public string ImportBlogPosts(string atomUrl, int mediaFolderId, int blogRootId)
    {
	    var mediaService = DependencyResolver.Current.GetService<IMediaService>();
      var formatter = new Atom10FeedFormatter();
      using (var reader = XmlReader.Create(atomUrl))
      {
        formatter.ReadFrom(reader);
      }

      foreach (var item in formatter.Feed.Items)
      {
				var completedMedia = new Dictionary<string, string>();
        var content = ((TextSyndicationContent) item.Content).Text;
				var ms = Regex.Matches(content, "(https?:)?//?[^'\"<>]+?\\.(jpg|jpeg|gif|png)", RegexOptions.IgnoreCase);
        foreach (var match in ms)
        {
          var url = match.ToString();
					var filename = Path.GetFileName(url);
	        if (!completedMedia.ContainsKey(filename))
	        {
		        try
		        {
			        var request = (HttpWebRequest) WebRequest.Create(url);
			        var response = request.GetResponse();
		          using (var stream = response.GetResponseStream())
		          {
		            using (var memStream = new MemoryStream())
		            {
                  int count;
                  do
                  {
                    byte[] buf = new byte[1024];
                    count = stream.Read(buf, 0, 1024);
                    memStream.Write(buf, 0, count);
                  } while (stream.CanRead && count > 0);

                  IMedia media = mediaService.CreateMedia(filename, mediaFolderId, "Image");
                  media.SetValue("umbracoFile", filename, memStream);
                  mediaService.Save(media);
                  var newFilename = media.GetValue<string>("umbracoFile");
                  content = content.Replace(url, newFilename);
                  completedMedia.Add(filename, newFilename);
		            }
		          }
	        
		        }
		        catch
		        {
			        // Do nothing
		        }


	        }
	        else
	        {
		        content = content.Replace(url, completedMedia[filename]);
	        }
        }

        var helper = new UmbracoHelper();
        var title = HttpUtility.HtmlDecode(item.Title.Text);

	      // Now create a post for the blog entry
        var postDic = new Dictionary<string, object>
        {
            {"postDate", item.PublishDate.DateTime},
            {"contentTitle", title},
            {"postSummary", helper.Truncate(content, 200).ToString()},
            {"navTitle", title},
            {"contentBody", content},
            {"metaTitle", title},
				    {"postTags", String.Join(",", item.Categories.Select(c => c.Name).ToArray())}
        };

				CreateContentNode(title, "BlogPost", postDic, blogRootId, false);
      }
      return "OK";
    }

    /// <summary>
    /// Creates a content node.
    /// </summary>
    /// <param name="nodeName">The node name</param>
    /// <param name="nodeTypeAlias">The nodeTypeAlias</param>
    /// <param name="properties">A dictionary of node properties</param>
    /// <param name="parentId">The parent node</param>
    /// <param name="publish">Publish or just save?</param>
    /// <returns>IContent</returns>
    public static IContent CreateContentNode(string nodeName, string nodeTypeAlias, Dictionary<string, object> properties, int parentId, bool publish)
    {
      var contentService = UmbracoContext.Current.Application.Services.ContentService;

      // create new content node
      var doc = contentService.CreateContent(nodeName, parentId, nodeTypeAlias);

      // load properties and saves
      return UpdateContentNode(doc, properties, publish);
    }

    /// <summary>
    /// Updates properties in a content node.
    /// </summary>
    /// <param name="doc">The IContent</param>
    /// <param name="properties">A dictionary of node properties</param>
    /// <param name="publish">Publish or just save?</param>
    /// <returns></returns>
    public static IContent UpdateContentNode(IContent doc, Dictionary<string, object> properties, bool publish)
    {
      var contentService = UmbracoContext.Current.Application.Services.ContentService;

      // save properties
      doc = UpdateContentProperties(doc, properties);

      // publish the node
      if (publish)
      {
        contentService.SaveAndPublishWithStatus(doc);
      }

      return doc;
    }

    /// <summary>
    /// Iterates over properties and loads them into the document properties. Calls Save() without raising events.
    /// </summary>
    /// <param name="doc">The document</param>
    /// <param name="properties">A dictionary of properties</param>
    /// <returns>The updated Document</returns>
    public static IContent UpdateContentProperties(IContent doc, Dictionary<string, object> properties)
    {
      var contentService = UmbracoContext.Current.Application.Services.ContentService;

      foreach (var key in properties.Keys)
      {
        doc.SetValue(key, properties[key]);
      }

      contentService.Save(doc, raiseEvents: false);
      return doc;
    }

  }
}
