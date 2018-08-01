using Comments.Services;
using Comments.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Comments.Controllers.Api
{
    [Produces("application/json")]
    [Route("consultations/api/[controller]")]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentsController> _logger;

        public CommentsController(ICommentService commentService, ILogger<CommentsController> logger)
        {
            _commentService = commentService;
            _logger = logger;
        }

		/// <summary>
		/// GET: eg. consultations/api/Comments?sourceURI=%2Fconsultations%2F1%2F1%2Fchapter-slug
		/// </summary>
		/// <param name="sourceURI">this is really the relativeURL eg "/1/1/introduction" on document page or "/1/review" on review page</param>
		/// <returns></returns>
		[HttpGet]
        public CommentsAndQuestions Get(string sourceURI)
        {
            if (string.IsNullOrWhiteSpace(sourceURI))
                throw new ArgumentNullException(nameof(sourceURI));

            return _commentService.GetCommentsAndQuestions(relativeURL: sourceURI);
        }
	}
}
