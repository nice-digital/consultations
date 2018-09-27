using Comments.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Comments.Controllers.Api
{
	/// <summary>
	/// This is just here so the front-end can log stuff to kibana.
	/// the front-end can call this either during the server-side render, or the client-side render (or after initial render) basically anytime. 
	/// </summary>
    [Produces("application/json")]
    [Route("consultations/api/[controller]")]
    public class LoggingController : ControllerBase
    {
        private readonly ILogger<LoggingController> _logger;
        public LoggingController(ILogger<LoggingController> logger)
        {
            _logger = logger;
        }

        // POST: consultations/api/Logging
        [HttpPost]
        public JsonResult PostLog([FromBody] string message, LogLevel logLevel = LogLevel.Warning) 
        {

			_logger.Log(logLevel, message);



	        return Json(new { success = true});
        }
    }
}
