using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Comments.Controllers.Api
{
	
	public class LogPostModel
	{
		public string Message { get; set; }
	}

	/// <summary>
	/// This is just here so the front-end can log stuff to kibana.
	/// the front-end can call this either during the server-side render or client-side.
	/// </summary>
	[Produces("application/json")]
	[ApiExplorerSettings(IgnoreApi = true)]
	[Route("consultations/api/[controller]")]
    public class LoggingController : ControllerBase
    {
        private readonly ILogger<LoggingController> _logger;
        public LoggingController(ILogger<LoggingController> logger)
        {
            _logger = logger;
        }

		/// <summary>
		/// POST: consultations/api/Logging?logLevel=Error
		/// </summary>
		/// <param name="logPost">LogPostModel</param>
		/// <param name="logLevel">LogLevel</param>
		/// <returns></returns>
		[HttpPost]
        public JsonResult PostLog([FromBody] LogPostModel logPost, [JsonConverter(typeof(StringEnumConverter))] LogLevel logLevel = LogLevel.Warning)
        {
			_logger.Log(logLevel, logPost.Message);
	        return Json(new { success = true});
        }
	}
}
