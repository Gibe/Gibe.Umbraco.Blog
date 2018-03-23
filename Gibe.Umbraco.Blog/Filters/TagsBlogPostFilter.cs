using System.Collections.Generic;
using System.Linq;
using Examine.SearchCriteria;

namespace Gibe.Umbraco.Blog.Filters
{
	public class TagsBlogPostFilter : IBlogPostFilter
	{
		private readonly IEnumerable<string> _tags;

		public TagsBlogPostFilter(IEnumerable<string> tags)
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