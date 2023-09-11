using Examine.Search;

namespace Gibe.Umbraco.Blog.Filters
{
	public class SectionBlogPostFilter : IBlogPostFilter
	{
		public int SectionNodeId { get; }

		public SectionBlogPostFilter(int sectionNodeId)
		{
			SectionNodeId = sectionNodeId;
		}

		public IBooleanOperation GetCriteria(IQuery query)
		{
			return query.Field(ExamineFields.Path, SectionNodeId.ToString());
		}
	}
}