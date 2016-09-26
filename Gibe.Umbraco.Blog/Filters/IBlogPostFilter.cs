using System.Collections.Generic;
using Examine.SearchCriteria;
using Umbraco.Core.Models;

namespace Gibe.Umbraco.Blog.Filters
{
	public interface IBlogPostFilter
	{
		IBooleanOperation GetCriteria(IQuery query);
	}
}