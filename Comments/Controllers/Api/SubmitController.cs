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

		public SubmitController(ISubmitService submitService, ILogger<SubmitController> logger)
	    {
		    _submitService = submitService;
		    _logger = logger;
		}

		// POST: consultations/api/submit
		[HttpPost]
	    public IActionResult PostSubmit([FromBody] ViewModels.CommentsAndAnswers commentsAndAnswers)
	    {
			//TODO: it should save any unsaved comments before submitting
		    if (!ModelState.IsValid)
		    {
			    return BadRequest(ModelState);
		    }

		    var result = _submitService.SubmitCommentsAndAnswers(commentsAndAnswers);
		    var invalidResult = Validate(result.validate, _logger);

		    return invalidResult ?? Ok(result.rowsUpdated);
		}
	}
}
