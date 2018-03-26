using System.Collections.Generic;
using System.Linq;
using Examine.SearchCriteria;

namespace Gibe.Umbraco.Blog.Filters
{
	public class AtLeastOneMatchingTagFilter : IBlogPostFilter
	{
		private readonly IEnumerable<string> _tags;

		public AtLeastOneMatchingTagFilter(IEnumerable<string> tags)
		{
			_tags = tags;
		}
		
		public IBooleanOperation GetCriteria(IQuery query)
		{
			return query.GroupedOr(new []{"tag"}, _tags.ToArray());
		}

		public IEnumerable<string> Tags => _tags;
	}
}