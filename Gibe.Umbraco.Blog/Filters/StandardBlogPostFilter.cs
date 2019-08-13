using Examine.Search;

namespace Gibe.Umbraco.Blog.Filters
{
	public class StandardBlogPostFilter : IBlogPostFilter
	{
		private readonly string _propertyName;
		private readonly string _propertyValue;

		public StandardBlogPostFilter(string propertyName, string value)
		{
			_propertyName = propertyName;
			_propertyValue = value;
		}
		
		public IBooleanOperation GetCriteria(IQuery query)
		{
			return query.Field(_propertyName, _propertyValue);
		}
	}
}