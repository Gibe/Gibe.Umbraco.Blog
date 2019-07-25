using Examine.Search;

namespace Gibe.Umbraco.Blog.Filters
{
	public class DateBlogPostFilter : IBlogPostFilter
	{
		private readonly int _year;
		private readonly int? _month;
		private readonly int? _day;

		public DateBlogPostFilter(int year, int? month, int? day)
		{
			_year = year;
			_month = month;
			_day = day;
		}
		
		public IBooleanOperation GetCriteria(IQuery query)
		{
			var output = query.Field("postDateYear", _year.ToString("00"));

			if (_month.HasValue)
			{
				output = output.And().Field("postDateMonth", _month.Value.ToString("00"));
			}
			if (_day.HasValue)
			{
				output = output.And().Field("postDateDay", _day.Value.ToString("00"));
			}
			return output;
		}
	}
}