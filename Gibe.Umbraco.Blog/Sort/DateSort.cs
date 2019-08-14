using Examine.Search;

namespace Gibe.Umbraco.Blog.Sort
{
	public class DateSort : ISort
	{
		private readonly bool _descending;

		public DateSort(bool descending = true)
		{
			_descending = descending;
		}

		public IOrdering GetCriteria(IBooleanOperation query)
		{
			if (_descending)
			{
				return query.And().All().OrderByDescending(new SortableField(ExamineFields.PostDateTicks, SortType.Long));
			}
			return query.And().All().OrderBy(new SortableField(ExamineFields.PostDateTicks, SortType.Long));
		}
	}
}
