using Examine.SearchCriteria;
using Gibe.Umbraco.Blog.Models;

namespace Gibe.Umbraco.Blog.Filters
{
	public class TagBlogPostFilter : IBlogPostFilter
	{
		private readonly string _tag;

		public TagBlogPostFilter(string tag)
		{
			_tag = tag;
		}
		
		public IBooleanOperation GetCriteria(IQuery query)
		{
			// TODO : Need to put tags individually into search index
			// TODO : Not hardcode
			return query.Field("settingsNewsTags", _tag);
		}
	}
}