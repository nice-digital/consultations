using Comments.Services;
using Comments.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Comments.Controllers.Api
{
    [Produces("application/json")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentsController> _logger;

        public CommentsController(ICommentService commentService, ILogger<CommentsController> logger)
        {
            _commentService = commentService;
            _logger = logger;
		}

		// GET: eg. consultations/api/Comments?sourceURI=%2Fconsultations%2F1%2F1%2Fchapter-slug
		/// <summary>
		/// Returns a list of comments for a given URI, restricted by current user
		/// </summary>
		/// <param name="sourceURI">this is really the relativeURL eg "/1/1/introduction" on document page</param>
		/// <returns>CommentsAndQuestions</returns>
		[Route("consultations/api/[controller]")]
		[HttpGet]
		[ProducesResponseType(typeof(CommentsAndQuestions), StatusCodes.Status200OK)]
		public CommentsAndQuestions Get(string sourceURI)
        {
            if (string.IsNullOrWhiteSpace(sourceURI))
                throw new ArgumentNullException(nameof(sourceURI));

            return _commentService.GetCommentsAndQuestions(relativeURL: sourceURI);
		}

		// GET: eg. consultations/api/CommentsForReview?relativeURL=%2F1%2Freview
		/// <summary>
		/// Returns a list of comments for a given URI, restricted by current user. For the review page.
		/// </summary>
		/// <param name="relativeURL">this is the relativeURL eg "/1/review" on review page</param>
		/// <param name="reviewPageViewModel"></param>
		/// <returns>ReviewPageViewModel</returns>
		[Route("consultations/api/[controller]ForReview")]
	    [HttpGet]
		[ProducesResponseType(typeof(ReviewPageViewModel), StatusCodes.Status200OK)]
		public ReviewPageViewModel Get(string relativeURL, ReviewPageViewModel reviewPageViewModel)
	    {
			if (string.IsNullOrWhiteSpace(relativeURL))
				throw new ArgumentNullException(nameof(relativeURL));

			return _commentService.GetCommentsAndQuestionsForReview(relativeURL, reviewPageViewModel);
	    }

	}
}
