using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comments.Controllers.Api
{
    [Route("consultations/api/[controller]")]
    [Authorize]
    public class TestController : Controller
    {
        /// <summary>
        /// GET: /consultations/api/Test
        /// 
        /// this controller is here temporarily to test the Auth.
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Content("this is the test controller.");
        }
    }
}