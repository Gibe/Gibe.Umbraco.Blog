using Examine.Search;
using Gibe.Umbraco.Blog.Utilities;

namespace Gibe.Umbraco.Blog.Filters
{
	public class CategoryBlogPostFilter : IBlogPostFilter
	{
		private readonly string _categoryName;

		public CategoryBlogPostFilter(string categroyName)
		{
			_categoryName = categroyName;
		}

		public IBooleanOperation GetCriteria(IQuery query)
		{
			return query.Field(ExamineFields.CategoryName, new ExactPhraseExamineValue(_categoryName.ToLower()));
		}
	}
}
