using System;
using Comments.Common;
using Comments.Configuration;
using Comments.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Comments.Controllers.Api
{
	/// <summary>
	/// This Status controller isn't for the Status table. It's for the status health check page.
	/// </summary>
    [Produces("application/json")]
    [Route("consultations/api/[controller]")]
    public class StatusController : ControllerBase
    {
	    private readonly IStatusService _statusService;
	    private readonly ILogger<StatusController> _logger;
        public StatusController(IStatusService statusService, ILogger<StatusController> logger)
        {
	        _statusService = statusService;
	        _logger = logger;
        }

        // GET: consultations/api/Status?apiKey=not_a_real_api_thats_a_secret
        [HttpGet]
        public IActionResult GetStatus(string apiKey)
        {
			var apiKeyInRequest = Request.Headers[Constants.StatusAPIKeyName];
	        var apiKeyToValidate = apiKey ?? apiKeyInRequest; //use the querystring in preference to the header.
	        if (string.IsNullOrEmpty(apiKeyToValidate) ||
	            !string.Equals(apiKeyToValidate, AppSettings.StatusConfig.APIKey, StringComparison.OrdinalIgnoreCase))
	        {
		        return new UnauthorizedResult();
	        }

	        var result = _statusService.GetStatusModel();
            return Ok(result);
        }
    }
}
