﻿using System;
using Comments.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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

            return Ok(result);
        }

        // POST: consultations/api/Answer
        [HttpPost]
        public IActionResult PostAnswer([FromBody] ViewModels.Answer answer) 
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }

            var savedComment = _answerService.CreateAnswer(answer);

            return CreatedAtAction("GetAnswer", new { id = savedComment.AnswerId }, savedComment);
        }
    }
}
