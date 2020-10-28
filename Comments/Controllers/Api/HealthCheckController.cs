using Comments.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Comments.Controllers.Api
{
	[Produces("application/json")]
    [Route("consultations/healthcheckapi")]
    public class HealthCheckController : ControllerBase
    {
	    private readonly IStatusService _statusService;
	    private readonly ILogger<HealthCheckController> _logger;
        public HealthCheckController(IStatusService statusService, ILogger<HealthCheckController> logger)
        {
	        _statusService = statusService;
	        _logger = logger;
        }

        public class HealthStatus
        {
	        public HealthStatus(bool healthy)
	        {
		        Status = healthy ? "Healthy" : "Unhealthy";
	        }
	        public string Status { get;  }
			public object Entries => new object(); //the entries object is required by HealthChecksUI, but can be empty.
        }

		// GET: consultations/healthcheckapi
		[HttpGet]
        public IActionResult Get()
        {
	        var statusModel = _statusService.GetStatusModel();
			if (statusModel.TotalComments > 0)
			{
				return Json(new HealthStatus(healthy: true));
			}
			return Json(new HealthStatus(healthy: false));
        }
    }
}
