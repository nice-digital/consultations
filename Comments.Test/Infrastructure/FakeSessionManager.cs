using Microsoft.FeatureManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comments.Test.Infrastructure
{
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
