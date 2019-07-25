using Examine;
using Examine.Providers;
using Lucene.Net.Documents;
using System;
using System.Globalization;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Services;

namespace Gibe.Umbraco.Blog.Composing
{
	public class IndexEvents : IComponent
	{
		private readonly IExamineManager _examineManager;
		private readonly IProfilingLogger _profilingLogger;
		private readonly ILocalizationService _localizationService;

		public IndexEvents(IExamineManager examineManager,
			IProfilingLogger profilingLogger,
			ILocalizationService localizationService)
		{
			_examineManager = examineManager;
			_profilingLogger = profilingLogger;
			_localizationService = localizationService;
		}

		public void Initialize()
		{
			_examineManager.TryGetIndex("ExternalIndex", out var index);

			((BaseIndexProvider)index).TransformingIndexValues += ExternalIndexTransformingIndexValues;
		}

		private void ExternalIndexTransformingIndexValues(object sender, IndexingItemEventArgs e)
		{

		}

		public void Terminate()
		{
			
		}
	}
}
