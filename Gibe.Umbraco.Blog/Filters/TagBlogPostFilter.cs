using Examine.Search;

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
			return query.ManagedQuery(_tag.ToLower(), new[] {ExamineFields.Tag});
		}		
	}
}