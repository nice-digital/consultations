using Comments.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Comments.Controllers.Api
{
    [Produces("application/json")]
    [Route("consultations/api/[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        private readonly ILogger<QuestionController> _logger;
        public QuestionController(IQuestionService questionService, ILogger<QuestionController> logger)
        {
            _questionService = questionService;
            _logger = logger;
        }

        // GET: consultations/api/Question/5
        [HttpGet("{questionId}")]
        public IActionResult GetQuestion([FromRoute] int questionId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _questionService.GetQuestion(questionId);

            var invalidResult = Validate(result.validate, _logger);

            return invalidResult ?? Ok(result.question);
        }

        // POST: consultations/api/Question
        [HttpPost]
        [Authorize(Policy = "Administrator,CommentAdminTeam,IndevUser")]
		public IActionResult PostQuestion([FromBody] ViewModels.Question question)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _questionService.CreateQuestion(question);
            var invalidResult = Validate(result.validate, _logger);

            return invalidResult ?? CreatedAtAction("GetQuestion", new { questionId = result.question.QuestionId }, result.question);
        }

        // PUT: consultations/api/Question/5
        [HttpPut("{questionId}")]
        [Authorize(Policy = "Administrator,CommentAdminTeam,IndevUser")]
		public IActionResult PutQuestion([FromRoute] int questionId, [FromBody] ViewModels.Question question)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (questionId != question.QuestionId)
            {
                return BadRequest();
            }

            var result = _questionService.EditQuestion(questionId, question);
            var invalidResult = Validate(result.validate, _logger);

            return invalidResult ?? Ok(question);
        }

        // DELETE: consultations/api/Question/5
        [HttpDelete("{QuestionId}")]
        [Authorize(Policy = "Administrator,CommentAdminTeam,IndevUser")]
		public IActionResult DeleteQuestion([FromRoute] int questionId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _questionService.DeleteQuestion(questionId);
            var invalidResult = Validate(result.validate, _logger);

            return invalidResult ?? Ok(result.rowsUpdated);
        }
    }
}
