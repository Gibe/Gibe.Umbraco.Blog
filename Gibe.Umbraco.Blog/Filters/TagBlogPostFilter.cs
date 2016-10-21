using Examine.SearchCriteria;
using Gibe.Umbraco.Blog.Models;
using Gibe.Umbraco.Blog.Utilities;

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
			return query.Field("tag", new ExactPhraseExamineValue(_tag.ToLower()));
		}

		
	}
}