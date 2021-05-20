using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.FeatureManagement;

namespace Comments.Test.Infrastructure
{
	public class FakeFeatureManager : IFeatureManager
	{
		private readonly Dictionary<string, bool> _features;

		public FakeFeatureManager(Dictionary<string, bool> features = null)
		{
			_features = features;
		}

		public IAsyncEnumerable<string> GetFeatureNamesAsync()
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
}
