using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Examine.SearchCriteria;

namespace Gibe.Umbraco.Blog.Sort
{
	public class DateSort : ISort
	{
		private readonly bool _descending;

		public DateSort(bool descending = true)
		{
			_descending = descending;
		}

		public IBooleanOperation GetCriteria(IBooleanOperation query)
		{
			if (_descending)
			{
				return query.And().OrderByDescending("postDate");
			}
			return query.And().OrderBy("postDate");
		}
	}
}
