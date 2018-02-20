using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Examine.SearchCriteria;

namespace Gibe.Umbraco.Blog.Sort
{
	public interface ISort
	{
		IBooleanOperation GetCriteria(IBooleanOperation query);
	}
}
