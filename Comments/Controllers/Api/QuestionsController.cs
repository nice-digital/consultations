using System.Threading.Tasks;
using Comments.Services;
using Microsoft.AspNetCore.Authorization;
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
		[HttpGet]
        public async Task<IActionResult> GetQuestions(int consultationId, bool draft, string reference)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _questionService.GetQuestionAdmin(consultationId, draft, reference);

            return Ok(result);
        }
    }
}
