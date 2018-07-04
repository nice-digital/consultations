using Comments.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Comments.Controllers.Api
{
	[Produces("application/json")]
	[Route("consultations/api/[controller]")]
	[Authorize]
	public class SubmitController : ControllerBase
	{
		private readonly IConsultationService _consultationService;
		private readonly ICommentService _commentService;
	    private readonly ILogger<SubmitController> _logger;

		public SubmitController(IConsultationService consultationService, ICommentService commentService, ILogger<SubmitController> logger)
	    {
		    _consultationService = consultationService;
		    _commentService = commentService;
		    _logger = logger;
		}

		// POST: consultations/api/submit
		[HttpPost]
	    public IActionResult Post([FromBody] ViewModels.CommentsAndAnswers commentsAndAnswers)
	    {
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var result = _consultationService.SubmitCommentsAndAnswers(commentsAndAnswers);
			var invalidResult = Validate(result.validate, _logger);
			
			return invalidResult ?? Ok(commentsAndAnswers); //should return comments and answers, might need submission object too
	    }
	}
}
