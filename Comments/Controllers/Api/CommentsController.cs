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
        /// GET: eg. consultations/api/Comments?consultationId=1&documentId=1&chapterSlug=chapter-slug
        /// </summary>
        /// <param name="consultationId"></param>
        /// <param name="documentId">if null, show first commentable document.</param>
        /// <param name="chapterSlug">nullable. if not, show first chapter.</param>
        /// <returns></returns>
        [HttpGet]
        public CommentsAndQuestions Get(int consultationId, int documentId, string chapterSlug)
        {
            if (string.IsNullOrWhiteSpace(chapterSlug))
                throw new ArgumentNullException(nameof(chapterSlug));

            return _commentService.GetCommentsAndQuestions(consultationId, documentId, chapterSlug);
        }
    }
}
