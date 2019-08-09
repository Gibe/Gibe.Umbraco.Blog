using Examine.Search;

namespace Gibe.Umbraco.Blog.Filters
{
	public class DateBlogPostFilter : IBlogPostFilter
	{
		private readonly int _year;
		private readonly int? _month;
		private readonly int? _day;

		public DateBlogPostFilter(int year, int? month = null, int? day = null)
		{
			_year = year;
			_month = month;
			_day = day;
		}
		
		public IBooleanOperation GetCriteria(IQuery query)
		{
			var output = query.Field(ExamineFields.PostDateYear, _year.ToString("00"));

			if (_month.HasValue)
			{
				output = output.And().Field(ExamineFields.PostDateMonth, _month.Value.ToString("00"));
			}
			if (_day.HasValue)
			{
				output = output.And().Field(ExamineFields.PostDateDay, _day.Value.ToString("00"));
			}
			return output;
		}
	}
}