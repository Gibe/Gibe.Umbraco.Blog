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
			return query.Field("tag", _tag);
		}
	}
}