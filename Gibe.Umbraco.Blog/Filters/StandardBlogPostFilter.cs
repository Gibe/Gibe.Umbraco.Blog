﻿using Examine.Search;

namespace Gibe.Umbraco.Blog.Filters
{
	public class StandardBlogPostFilter : IBlogPostFilter
	{
		public string PropertyName { get; set; }
		public string PropertyValue { get; set; }

		public StandardBlogPostFilter() { }

		public StandardBlogPostFilter(string propertyName, string value)
		{
			PropertyName = propertyName;
			PropertyValue = value;
		}

		public IBooleanOperation GetCriteria(IQuery query)
		{
			return query.Field(PropertyName, PropertyValue);
		}
	}
}