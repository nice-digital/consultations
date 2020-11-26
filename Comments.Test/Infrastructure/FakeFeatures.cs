using Microsoft.FeatureManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comments.Test.Infrastructure
{
	public class FakeFeatureManager : IFeatureManager
	{
		private readonly Dictionary<string, bool> _features;

		public FakeFeatureManager(Dictionary<string, bool> features = null)
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

			if (!_features.ContainsKey(feature))
			{
				return Task.Run(() => false);
			}

			return Task.Run(() => _features[feature]);

		}

		public Task<bool> IsEnabledAsync<TContext>(string feature, TContext context)
		{
			throw new NotImplementedException();
		}
	}

	public class FakeSessionManager : ISessionManager
	{
		private readonly Dictionary<string, bool> _features;

		public FakeSessionManager(Dictionary<string, bool> features = null)
		{
			_features = features;
		}

		public Task SetAsync(string featureName, bool enabled)
		{
			throw new NotImplementedException();
		}

		public Task<bool?> GetAsync(string featureName)
		{
			if (_features == null)
				return Task.Run(() => (bool?)false);

			if (!_features.ContainsKey(featureName))
			{
				return Task.Run(() => (bool?)false);
			}

			return Task.Run(() => (bool?)_features[featureName]);
		}
	}
}
