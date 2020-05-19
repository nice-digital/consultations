using Comments.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Comments.Controllers.Api
{
    [Produces("application/json")]
    [Route("consultations/api/[controller]")]
    [Authorize] //TODO: validate this works!
	public class AnswerController : ControllerBase
    {
        private readonly IAnswerService _answerService;
        private readonly ILogger<AnswerController> _logger;
        public AnswerController(IAnswerService answerService, ILogger<AnswerController> logger)
        {
            _answerService = answerService;
            _logger = logger;
        }

        // GET: consultations/api/Answer/5 
        [HttpGet("{answerId}")]
        public IActionResult GetAnswer([FromRoute] int answerId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _answerService.GetAnswer(answerId);

            if (result.answer == null)
            {
                return NotFound();
            }

            var invalidResult = Validate(result.validate, _logger);
            return invalidResult ?? Ok(result.answer);
        }

        // POST: consultations/api/Answer
        [HttpPost]
        public IActionResult PostAnswer([FromBody] ViewModels.Answer answer) 
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            var result = _answerService.CreateAnswer(answer);
            var invalidResult = Validate(result.validate, _logger);

            return invalidResult ?? CreatedAtAction("GetAnswer", new { answerId = result.answer.AnswerId }, result.answer);
        }

        // PUT: consultations/api/Answer/5
        [HttpPut("{answerId}")]
        public IActionResult PutAnswer([FromRoute] int answerId, [FromBody] ViewModels.Answer answer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (answerId != answer.AnswerId)
            {
                return BadRequest();
            }

            var result = _answerService.EditAnswer(answerId, answer);
            var invalidResult = Validate(result.validate, _logger);

            return invalidResult ?? Ok(answer);
        }

        // DELETE: consultations/api/Answer/5
        [HttpDelete("{answerId}")]
        public IActionResult DeleteAnswer([FromRoute] int answerId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var result = _answerService.DeleteAnswer(answerId);
            var invalidResult = Validate(result.validate, _logger);

            return invalidResult ?? Ok(result.rowsUpdated);
        }
    }
}
