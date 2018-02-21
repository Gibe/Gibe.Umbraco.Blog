using Examine.SearchCriteria;
using Gibe.Umbraco.Blog.Utilities;

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
			return query.Field("postAuthorName", new ExactPhraseExamineValue(_author.ToLower()));
		}
	}
}