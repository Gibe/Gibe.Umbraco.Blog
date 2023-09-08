using System.Collections.Generic;
using System.Linq;
using Examine.Search;

namespace Gibe.Umbraco.Blog.Filters
{
	public class AtLeastOneMatchingTagFilter : IBlogPostFilter
	{
		public IEnumerable<string> Tags { get; }

		public AtLeastOneMatchingTagFilter() { }

		public AtLeastOneMatchingTagFilter(IEnumerable<string> tags)
		{
			Tags = tags?.Select(t => t.ToLower()).ToList(); ;
		}

		public IBooleanOperation GetCriteria(IQuery query)
		{
			return query.GroupedOr(new[] { ExamineFields.Tag }, Tags.ToArray());
		}
	}
}