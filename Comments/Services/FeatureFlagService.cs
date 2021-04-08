using Comments.Models;
using System.Collections.Generic;
using Comments.Common;
using Microsoft.FeatureManagement;
using System.Threading.Tasks;
using Microsoft.FeatureManagement.Mvc;

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

            featureFlags.Add(new KeyValuePair<string, bool>(Constants.Features.OrganisationalCommenting, await _featureManager.IsEnabledAsync(Constants.Features.OrganisationalCommenting)));
            featureFlags.Add(new KeyValuePair<string, bool>(Constants.Features.IndevUsingIDAMAuth, await _featureManager.IsEnabledAsync(Constants.Features.IndevUsingIDAMAuth)));

            return featureFlags;
		}
	}
}
