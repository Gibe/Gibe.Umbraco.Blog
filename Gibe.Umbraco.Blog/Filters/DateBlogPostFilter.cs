using Examine.Search;

namespace Gibe.Umbraco.Blog.Filters
{
	public class DateBlogPostFilter : IBlogPostFilter
	{
		public int Year { get; set; }
		public int? Month { get; set; }
		public int? Day { get; set; }

		public DateBlogPostFilter(int year, int? month = null, int? day = null)
		{
			Year = year;
			Month = month;
			Day = day;
		}

		public IBooleanOperation GetCriteria(IQuery query)
		{
			var output = query.Field(ExamineFields.PostDateYear, Year.ToString("00"));

			if (Month.HasValue)
			{
				output = output.And().Field(ExamineFields.PostDateMonth, Month.Value.ToString("00"));
			}
			if (Day.HasValue)
			{
				output = output.And().Field(ExamineFields.PostDateDay, Day.Value.ToString("00"));
			}
			return output;
		}
	}
}