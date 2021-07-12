using Examine.Search;

namespace Gibe.Umbraco.Blog.Filters
{
	public class AuthorBlogPostFilter : IBlogPostFilter
	{
		private readonly string _author;

		public AuthorBlogPostFilter(string author)
		{
			_author = author;
		}

		public IBooleanOperation GetCriteria(IQuery query)
		{
			return query.ManagedQuery(_author.ToLower(), new[] { ExamineFields.PostAuthorName });
		}
	}
}