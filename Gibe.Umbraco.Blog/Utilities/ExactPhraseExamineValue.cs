using Examine.Search;

namespace Gibe.Umbraco.Blog.Utilities
{
	public class ExactPhraseExamineValue : IExamineValue
	{
		public ExactPhraseExamineValue(string phrase)
		{
			Examineness = Examineness.Escaped;
			Value = $"{phrase}";
			Level = 1;
		}

		public Examineness Examineness { get; }
		public float Level { get; }
		public string Value { get; }
	}
}
