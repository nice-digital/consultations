using System;
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
		private readonly ISubmitService _submitService;
	    private readonly ILogger<SubmitController> _logger;

		public SubmitController(ISubmitService submitService, ICommentService commentService, ILogger<SubmitController> logger)
	    {
		    _submitService = submitService;
		    _logger = logger;
		}

		// POST: consultations/api/submit
		[HttpPost]
	    public IActionResult Post([FromBody] ViewModels.Submission submission)
	    {
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
		    if (!submission.HasTobaccoLinks && !string.IsNullOrWhiteSpace(submission.TobaccoDisclosure))
		    {
				throw new ArgumentException(nameof(submission.TobaccoDisclosure));
		    }

			var result = _submitService.Submit(submission);
			var invalidResult = Validate(result.validate, _logger);
			
			return invalidResult ?? Ok(submission); //should return comments and answers, might need submission object too
	    }
	}
}
