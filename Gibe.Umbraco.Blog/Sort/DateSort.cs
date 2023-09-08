using Examine.Search;

namespace Gibe.Umbraco.Blog.Sort
{
	public class DateSort : ISort
	{
		public bool Descending { get; set; }

		public DateSort() { }

		public DateSort(bool descending = true)
		{
			Descending = descending;
		}

		public IOrdering GetCriteria(IBooleanOperation query)
		{
			if (Descending)
			{
				return query.And().All().OrderByDescending(new SortableField(ExamineFields.PostDate, SortType.Long));
			}
			return query.And().All().OrderBy(new SortableField(ExamineFields.PostDate, SortType.Long));
		}
	}
}
