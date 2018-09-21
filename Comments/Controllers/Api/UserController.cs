using Comments.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Comments.Controllers.Api
{
	[Produces("application/json")]
    [Route("consultations/api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ICommentService _commentService;
	    private readonly IUserService _userService;
	    private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
	        _userService = userService;
	        _logger = logger;
        }

		// GET: consultations/api/User?returnURL=%2f1%2f1%2fintroduction
		[HttpGet]
        public IActionResult Get(string returnURL)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

	        var signInDetails = _userService.GetCurrentUserSignInDetails(returnURL);

            return Ok(signInDetails);
        }
    }
}
