using System;

namespace Gibe.Umbraco.Blog.Exceptions
{
	public class IndexNotFoundException : Exception
	{
		public IndexNotFoundException(string indexName) 
			: base($"Could not find an index with the name {indexName}")
		{ }
	}
}
