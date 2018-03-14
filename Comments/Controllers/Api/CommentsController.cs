using Comments.Services;
using Comments.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Comments.Controllers.Api
{
    [Produces("application/json")]
    [Route("consultations/api/[controller]")]
    public class CommentsController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentsController> _logger;

        public CommentsController(ICommentService commentService, ILogger<CommentsController> logger)
        {
            _commentService = commentService;
            _logger = logger;
        }

        /// <summary>
        /// GET: eg. consultations/api/Comments?sourceURL=http%3A%2F%2Fwww.nice.org.uk%2Fconsultations%2F1%2F1%2Fchapter-slug
        /// </summary>
        /// <param name="sourceURL"></param>
        /// <returns></returns>
        [HttpGet]
        public CommentsAndQuestions Get(string sourceURL)
        {
            if (string.IsNullOrWhiteSpace(sourceURL))
                throw new ArgumentNullException(nameof(sourceURL));

            return _commentService.GetCommentsAndQuestions(sourceURL);
        }
    }
}
