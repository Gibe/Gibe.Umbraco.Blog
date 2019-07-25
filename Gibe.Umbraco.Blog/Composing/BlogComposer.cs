using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Gibe.Umbraco.Blog.Composing
{
	public class BlogComposer : IUserComposer
	{
		public void Compose(Composition composition)
		{
			composition.Components().Append<IndexEvents>();
		}
	}
}
