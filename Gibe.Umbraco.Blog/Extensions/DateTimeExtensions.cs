using System;
using System.Globalization;

namespace Gibe.Umbraco.Blog.Extensions
{
	public static class DateTimeExtensions
	{
		public static DateTime ParseFromExamineField(this string fieldValue)
		{
			return DateTime.ParseExact(fieldValue.Substring(0, 10), "MM/dd/yyyy", CultureInfo.InvariantCulture);
		}
	}
}
