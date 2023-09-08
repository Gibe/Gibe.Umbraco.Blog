using Examine.Search;

namespace Gibe.Umbraco.Blog.Filters
{
	public class AuthorBlogPostFilter : IBlogPostFilter
	{
		public string Author { get; set; }

		public AuthorBlogPostFilter(string author)
		{
			Author = author;
		}

		public IBooleanOperation GetCriteria(IQuery query)
		{
			return query.ManagedQuery(Author.ToLower(), new[] { ExamineFields.PostAuthorName });
		}
	}
}