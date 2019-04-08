using Comments.Services;
using Comments.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Comments.Controllers.Api
{
	[Authorize(Roles = "Administrator,CommentAdminTeam,IndevUser")]
	[Produces("application/json")]
    [Route("consultations/api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        private readonly ILogger<QuestionController> _logger;
        public QuestionsController(IQuestionService questionService, ILogger<QuestionController> logger)
        {
            _questionService = questionService;
            _logger = logger;
        }

		// GET: consultations/api/Questions?consultationId=22
		/// <summary>
		/// Gets all questions for a given consulation
		/// </summary>
		/// <param name="consultationId">ID of the consultation</param>
		/// <param name="draft">Is the consultation a draft consultation</param>
		/// <param name="reference">If the consultation is a draft, consultation reference must be supplied</param>
		/// <returns>Returns the QuestionAdmin view model</returns>
		[HttpGet]
		[ProducesResponseType(typeof(QuestionAdmin), StatusCodes.Status200OK)]
		public IActionResult GetQuestions(int consultationId, bool draft, string reference)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _questionService.GetQuestionAdmin(consultationId, draft, reference);

            return Ok(result);
        }
    }
}
