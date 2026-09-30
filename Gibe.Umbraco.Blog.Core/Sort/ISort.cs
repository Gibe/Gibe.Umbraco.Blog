using Examine.Search;

namespace Gibe.Umbraco.Blog.Sort
{
	public interface ISort
	{
		IOrdering GetCriteria(IBooleanOperation query);
	}
}
