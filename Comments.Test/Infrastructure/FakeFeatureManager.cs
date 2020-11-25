using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Comments.Common;
using Microsoft.FeatureManagement;

namespace Comments.Test.Infrastructure
{
	public class FakeFeatureManager : IFeatureManager
	{
		private readonly Dictionary<Features, bool> _features;

		public FakeFeatureManager(Dictionary<Features, bool> features = null)
		{
			_features = features;
		}

		public System.Collections.Generic.IAsyncEnumerable<string> GetFeatureNamesAsync()
		{
			throw new NotImplementedException();
		}

		public Task<bool> IsEnabledAsync(string feature)
		{
			if (_features == null)
				return Task.Run(() => false);

			Features parsedFeature;
			if (!Features.TryParse(feature, ignoreCase: true, out parsedFeature))
			{
				return Task.Run(() => false);
			}
			
			if (!_features.ContainsKey(parsedFeature))
			{
				return Task.Run(() => false);
			}

			return Task.Run(() => _features[parsedFeature]);

		}

		public Task<bool> IsEnabledAsync<TContext>(string feature, TContext context)
		{
			throw new NotImplementedException();
		}
	}
}
