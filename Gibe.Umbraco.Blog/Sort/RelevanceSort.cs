using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Examine.SearchCriteria;

namespace Gibe.Umbraco.Blog.Sort
{
	public class RelevanceSort : ISort
	{
		public IBooleanOperation GetCriteria(IBooleanOperation query)
		{
			return query;
		}
	}
}
