using Comments.Services;
using Comments.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Comments.Controllers.Api
{
    [Produces("application/json")]
    [Route("consultations/api/[controller]")]
    public class ReviewController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<ConsultationController> _logger;

        public ReviewController(ICommentService commentService, ILogger<ConsultationController> logger)
        {
            _commentService = commentService;
            _logger = logger;
        }

        /// <summary>
        /// GET: eg. consultations/api/Review?sourceURI=%2Fconsultations%2F1%2F1%2Fchapter-slug
        /// </summary>
        /// <param name="sourceURI">this is really the relativeURL eg "/1/1/introduction"</param>
        /// <returns></returns>
        [HttpGet]
        public CommentsAndQuestions Get(string sourceURI)
        {
            if (string.IsNullOrWhiteSpace(sourceURI))
                throw new ArgumentNullException(nameof(sourceURI));

            return _commentService.GetUsersCommentsAndQuestionsForConsultation(relativeURL: sourceURI);
        }
    }
}
