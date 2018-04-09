using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Comments.Controllers.Web
{
    /// <summary>
    /// This controller maps to the root of the app, i.e. "/".
    /// it will only get hit on local dev machines. On the server, the root of the app is served by orchard via varnish.
    /// 
    /// this controller is important for local dev, as without it nice accounts won't work. 
    /// 
    /// the reason for that is that nice accounts posts to the root of the website after logging in, then redirects after that. 
    /// </summary>
    [Route("")]
    public class RootController : Controller
    {
        private readonly ILogger<RootController> _logger;

        public RootController(ILogger<RootController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogWarning("Hitting the root controller. this should only occur locally.");
            //return Content("root controller hit");
            return Redirect("/consultations/1/1/introduction");
        }
    }
}