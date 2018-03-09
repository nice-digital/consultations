using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Comments.Controllers.Web
{
    [Route("consultations/test")]
    public class ConsultationsController : Controller
    {
        private readonly ILogger<ConsultationsController> _logger;

        public ConsultationsController(ILogger<ConsultationsController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogWarning("fyi: hitting the consultations controller");

            return View("Index");
        }
    }
}