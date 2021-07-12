using Examine;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Gibe.Umbraco.Blog.Extensions
{
	public static class ValueSetExtensions
	{
		public static string GetSingleValue(this ValueSet valueSet, string key) 
		{
			List<object> values;
			if (!valueSet.Values.TryGetValue(key, out values))
			{
				return null;
			}

			return values.FirstOrDefault().ToString();
		}

		public static T GetSingleValue<T>(this ValueSet valueSet, string key)
		{
			var value = GetSingleValue(valueSet, key);

			if (value == null)
			{
				return default(T);
			}

			var converter = TypeDescriptor.GetConverter(typeof(T));
			return (T)converter.ConvertFrom(value);
		}

		public static void TryAddOrAppend(this ValueSet valueSet, string key, object value)
		{
			var added = valueSet.TryAdd(key, value);

			if (added)
			{
				return;
			}

			valueSet.Values[key].Add(value);
		}
	}
}
