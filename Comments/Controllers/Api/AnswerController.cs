using System;
using Comments.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Comments.ViewModels;

namespace Comments.Controllers.Api
{
    [Produces("application/json")]
    [Route("consultations/api/[controller]")]
    public class AnswerController : Controller
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

            var invalidResult = Validate(result.validate);
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

            //var savedComment = _answerService.CreateAnswer(answer);

            //return CreatedAtAction("GetAnswer", new { id = savedComment.AnswerId }, savedComment);

            var result = _answerService.CreateAnswer(answer);
            var invalidResult = Validate(result.validate);

            return invalidResult ?? CreatedAtAction("GetAnswer", new { id = result.answer.AnswerId }, result.answer);
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
            var invalidResult = Validate(result.validate);

            return invalidResult ?? Ok(result.rowsUpdated);
        }

        private IActionResult Validate(Validate validate)
        {
            if (validate == null || validate.Valid)
                return null;

            _logger.LogWarning(validate.Message);

            if (validate.Unauthorised)
                return Unauthorized();

            if (validate.NotFound)
                return NotFound();

            return BadRequest();
        }
    }
}
