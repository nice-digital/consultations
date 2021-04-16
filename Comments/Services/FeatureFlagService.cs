using System.Collections.Generic;
using Comments.Common;
using Microsoft.FeatureManagement;
using System.Threading.Tasks;

namespace Comments.Services
{
	public interface IFeatureFlagService
	{
        Task<List<KeyValuePair<string, bool>>> GetFeatureFlags();
	}

	public class FeatureFlagService : IFeatureFlagService
	{
        private readonly IFeatureManager _featureManager;
        public FeatureFlagService(IFeatureManager featureManager)
		{
            _featureManager = featureManager;
        }

		public async Task<List<KeyValuePair<string, bool>>> GetFeatureFlags()
        {
            var featureFlags = new List<KeyValuePair<string, bool>>();

            var flags = typeof(Constants.Features).GetFields();

            foreach (var flag in flags)
            {
                var featureFlag = flag.Name.ToString();
                featureFlags.Add(new KeyValuePair<string, bool>(featureFlag, await _featureManager.IsEnabledAsync(featureFlag)));
            }
            
            return featureFlags;
		}
	}
}
