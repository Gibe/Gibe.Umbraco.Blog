using Examine.Search;

namespace Gibe.Umbraco.Blog.Filters
{
	public class CategoryBlogPostFilter : IBlogPostFilter
	{
		public string CategoryName { get; set; }

		public CategoryBlogPostFilter() { }

		public CategoryBlogPostFilter(string categoryName)
		{
			CategoryName = categoryName;
		}

		public IBooleanOperation GetCriteria(IQuery query)
		{
			return query.ManagedQuery(CategoryName.ToLower(), new[] { ExamineFields.CategoryName });
		}
	}
}
