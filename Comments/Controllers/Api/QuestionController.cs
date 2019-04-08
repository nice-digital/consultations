using Comments.Services;
using Comments.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Comments.Controllers.Api
{
	/// <summary>
	/// Operations for a single question
	/// </summary>
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
        /// <summary>
        /// Get single question from id
        /// </summary>
        /// <param name="questionId">id of the question</param>
        /// <returns>Returns the question view model</returns>
        [HttpGet("{questionId}")]
        [ProducesResponseType(typeof(Question), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(SerializableError), StatusCodes.Status404NotFound)]
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
	    /// <summary>
	    /// Adds a single question
	    /// </summary>
	    /// <param name="question">question view model</param>
	    /// <returns>Returns the question view model and Validate model</returns>
		[HttpPost]
		[ProducesResponseType(typeof((Question, Validate)), StatusCodes.Status200OK)]
		public IActionResult PostQuestion([FromBody] ViewModels.Question question)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _questionService.CreateQuestion(question);
            var invalidResult = Validate(result.validate, _logger);

            return invalidResult ?? CreatedAtAction("GetQuestion", new { id = result.question.QuestionId }, result.question);
        }

		// PUT: consultations/api/Question/5
		/// <summary>
		/// Updates an individual question based on question Id
		/// </summary>
		/// <param name="questionId">ID of the question to be updated</param>
		/// <param name="question">The values the question is to be updated with</param>
		/// <returns>Returns the question view model of the updated question</returns>
		[HttpPut("{questionId}")]
		[ProducesResponseType(typeof(Question), StatusCodes.Status200OK)]
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
		/// <summary>
		/// Deletes an individual question specified by question ID
		/// </summary>
		/// <param name="questionId">Id of the question to be deleted</param>
		/// <returns>Returns the number of rows affected</returns>
		[HttpDelete("{QuestionId}")]
		[ProducesResponseType(typeof((int, Validate)), StatusCodes.Status200OK)]
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
