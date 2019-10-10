using System;
using Comments.Common;
using Comments.Configuration;
using Comments.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Comments.Controllers.Api
{
	/// <summary>
	/// This Status controller isn't for the Status table. It's for the status health check page.
	/// </summary>
	[ApiExplorerSettings(IgnoreApi = true)]
	[Produces("application/json")]
    [Route("consultations/api/[controller]")]
    public class StatusAPIController : ControllerBase
    {
	    private readonly IStatusService _statusService;
	    private readonly ILogger<StatusAPIController> _logger;
        public StatusAPIController(IStatusService statusService, ILogger<StatusAPIController> logger)
        {
	        _statusService = statusService;
	        _logger = logger;
        }

        // GET: consultations/api/Status
		// with a request header with the api key in.
        [HttpGet]
        public IActionResult GetStatus()
        {
			var apiKeyInRequest = Request.Headers[Constants.StatusAPIKeyName];
	        if (string.IsNullOrEmpty(apiKeyInRequest) ||
	            !string.Equals(apiKeyInRequest, AppSettings.StatusConfig.APIKey, StringComparison.OrdinalIgnoreCase))
	        {
		        return new UnauthorizedResult();
	        }

	        var statusModel = _statusService.GetStatusModel();
			_logger.LogWarning("Status Model: {@statusModel}", statusModel); //logs to kibana as warning because that's our minimum log level. should really be info / diag etc.
            return Json(statusModel);
        }
    }

	[Produces("application/json")]
	[Route("consultations/api/[controller]")]
	[Authorize(Roles = "Administrator")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public class StatusController : ControllerBase
	{
		private readonly IStatusService _statusService;
		private readonly ILogger<StatusController> _logger;
		public StatusController(IStatusService statusService, ILogger<StatusController> logger)
		{
			_statusService = statusService;
			_logger = logger;
		}

		// GET: consultations/api/Status
		[HttpGet]
		public IActionResult GetStatus(string apiKey)
		{
			var model = _statusService.GetStatusModel();
			return Json(model);
		}
	}
}
