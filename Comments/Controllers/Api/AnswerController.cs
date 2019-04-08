using Comments.Services;
using Comments.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Comments.Controllers.Api
{
    [Produces("application/json")]
    [Route("consultations/api/[controller]")]
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
		/// <summary>
		/// Get single answer from id
		/// </summary>
		/// <param name="answerId">id of the answer</param>
		/// <returns>Returns the answer view model</returns>
		[HttpGet("{answerId}")]
        [ProducesResponseType(typeof(Answer), StatusCodes.Status200OK)]
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
		/// <summary>
		/// Adds a single answer
		/// </summary>
		/// <param name="answer">answer view model</param>
		/// <returns>Returns the answer view model and Validate model</returns>
		[HttpPost]
		[ProducesResponseType(typeof((Answer, Validate)), StatusCodes.Status200OK)]
		public IActionResult PostAnswer([FromBody] ViewModels.Answer answer) 
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            var result = _answerService.CreateAnswer(answer);
            var invalidResult = Validate(result.validate, _logger);

            return invalidResult ?? CreatedAtAction("GetAnswer", new { id = result.answer.AnswerId }, result.answer);
        }

		// PUT: consultations/api/Answer/5
		/// <summary>
		/// Updates an individual answer based on answer Id
		/// </summary>
		/// <param name="answerId">ID of the answer to be updated</param>
		/// <param name="answer">The values the answer is to be updated with</param>
		/// <returns>Returns the answer view model of the updated answer</returns>
		[HttpPut("{answerId}")]
		[ProducesResponseType(typeof(Answer), StatusCodes.Status200OK)]
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
		/// <summary>
		/// Deletes an individual answer specified by answer Id
		/// </summary>
		/// <param name="answerId">Id of the answer to be deleted</param>
		/// <returns>Returns the number of rows affected</returns>
		[HttpDelete("{answerId}")]
		[ProducesResponseType(typeof((int, Validate)), StatusCodes.Status200OK)]
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
