using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comments.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Comments.Controllers.Api
{
    [Produces("application/json")]
    [Route("consultations/api/[controller]")]
    public class FeatureFlagController : Controller
    {
        private readonly IFeatureFlagService _featureFlagService;

        public FeatureFlagController(IFeatureFlagService featureFlagService)
        {
            _featureFlagService = featureFlagService;
        }

        // GET: consultations/api/FeatureFlags 
        public async Task<List<KeyValuePair<string, bool>>> GetFeatureFlag()
        {
            var hello = "hello";
            var result = await _featureFlagService.GetFeatureFlags();

            return result;
        }
    }
}
