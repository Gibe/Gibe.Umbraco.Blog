using Examine.Search;

namespace Gibe.Umbraco.Blog.Filters
{
	public class CategoryBlogPostFilter : IBlogPostFilter
	{
		private readonly string _categoryName;

		public CategoryBlogPostFilter(string categoryName)
		{
			_categoryName = categoryName;
		}

		public IBooleanOperation GetCriteria(IQuery query)
		{
			return query.ManagedQuery(_categoryName.ToLower(), new[] { ExamineFields.CategoryName });
		}
	}
}
