using System;
using System.Net;
using Comments.Common;
using Comments.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NICE.Identity.Authentication.Sdk.Domain;

namespace Comments.Controllers.Api
{
	[Produces("application/json")]

	[Authorize]
	public class SubmitController : ControllerBase
	{
		private readonly ISubmitService _submitService;
	    private readonly ILogger<SubmitController> _logger;
		private readonly IHostingEnvironment _hostingEnvironment;

		public SubmitController(ISubmitService submitService, ICommentService commentService, ILogger<SubmitController> logger, IHostingEnvironment hostingEnvironment)
	    {
		    _submitService = submitService;
		    _logger = logger;
		    _hostingEnvironment = hostingEnvironment;
	    }

		// POST: consultations/api/submit
		[HttpPost]
		[Route("consultations/api/[controller]")]
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

			//just some temporary debug here:
			_logger.LogWarning($"submitted using environment: {_hostingEnvironment.EnvironmentName}");
		    if (_hostingEnvironment.IsProduction())
		    {
			    _logger.LogWarning($"submitted a comment in production! environment name: {_hostingEnvironment.EnvironmentName}");
			}

			return invalidResult ?? Ok(submission); //should return comments and answers, might need submission object too
	    }

		// POST: consultations/api/submitToLead
		[HttpPost]
		[Route("consultations/api/[controller]ToLead")]
		[Authorize(AuthenticationSchemes = OrganisationCookieAuthenticationOptions.DefaultScheme + "," + AuthenticationConstants.AuthenticationScheme)]
		public IActionResult PostSubmitToLead([FromBody] ViewModels.Submission submission, string emailAddress)
	    {
		    if (!ModelState.IsValid)
		    {
			    return BadRequest(ModelState);
		    }
		   
		    var result = _submitService.SubmitToLead(submission, emailAddress);
		    var invalidResult = Validate(result.validate, _logger);

		    //just some temporary debug here:
		    _logger.LogWarning($"submitted using environment: {_hostingEnvironment.EnvironmentName}");
		    if (_hostingEnvironment.IsProduction())
		    {
			    _logger.LogWarning($"submitted a comment in production! environment name: {_hostingEnvironment.EnvironmentName}");
		    }

		    return invalidResult ?? Ok(submission); //should return comments and answers, might need submission object too
	    }
	}


}
