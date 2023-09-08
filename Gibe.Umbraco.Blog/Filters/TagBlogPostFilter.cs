using Examine.Search;

namespace Gibe.Umbraco.Blog.Filters
{
	public class TagBlogPostFilter : IBlogPostFilter
	{
		public string Tag { get; set; }

		public TagBlogPostFilter() { }

		public TagBlogPostFilter(string tag)
		{
			Tag = tag;
		}

		public IBooleanOperation GetCriteria(IQuery query)
		{
			return query.ManagedQuery(Tag.ToLower(), new[] { ExamineFields.Tag });
		}
	}
}