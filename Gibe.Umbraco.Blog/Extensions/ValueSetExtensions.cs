using Examine;
using System.Collections.Generic;
using System.Linq;

namespace Gibe.Umbraco.Blog.Extensions
{
	public static class ValueSetExtensions
	{
		public static T GetSingleValue<T>(this ValueSet valueSet, string key) where T : class
		{
			List<object> values;
			if (!valueSet.Values.TryGetValue(key, out values))
			{
				return default(T);
			}

			return values.FirstOrDefault() as T;
		}
	}
}
