using Examine.Search;

namespace Gibe.Umbraco.Blog.Filters
{
	public interface IBlogPostFilter
	{
		IBooleanOperation GetCriteria(IQuery query);
	}
}