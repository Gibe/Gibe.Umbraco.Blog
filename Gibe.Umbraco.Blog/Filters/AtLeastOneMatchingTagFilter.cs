﻿using System.Collections.Generic;
using System.Linq;
using Examine.Search;

namespace Gibe.Umbraco.Blog.Filters
{
	public class AtLeastOneMatchingTagFilter : IBlogPostFilter
	{
		public AtLeastOneMatchingTagFilter(IEnumerable<string> tags)
		{
			Tags = tags;
		}

		public IBooleanOperation GetCriteria(IQuery query)
		{
			return query.GroupedOr(new[] { ExamineFields.Tag }, Tags.ToArray());
		}

		public IEnumerable<string> Tags { get; }
	}
}