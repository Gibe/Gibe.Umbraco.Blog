using Examine.Search;

namespace Gibe.Umbraco.Blog.Sort
{
	public class RelevanceSort : ISort
	{
		public IOrdering GetCriteria(IBooleanOperation query)
		{
			return query;
		}
	}
}
