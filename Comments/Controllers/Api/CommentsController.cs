using Comments.Services;
using Comments.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

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

		/// <summary>
		/// GET: eg. consultations/api/Comments?sourceURI=%2Fconsultations%2F1%2F1%2Fchapter-slug
		/// </summary>
		/// <param name="sourceURI">this is really the relativeURL eg "/1/1/introduction" on document page</param>
		/// <returns></returns>
		[Route("consultations/api/[controller]")]
		[HttpGet]
        public CommentsAndQuestions Get(string sourceURI, bool externalResource)
        {
            if (string.IsNullOrWhiteSpace(sourceURI))
                throw new ArgumentNullException(nameof(sourceURI));

            return _commentService.GetCommentsAndQuestions(relativeURL: sourceURI, externalResource: externalResource);
        }

		/// <summary>
		/// GET: eg. consultations/api/CommentsForReview?relativeURL=%2F1%2Freview
		/// </summary>
		/// <param name="relativeURL">this is the relativeURL eg "/1/review" on review page</param>
		/// <param name="reviewPageViewModel"></param>
		/// <returns></returns>
		[Route("consultations/api/[controller]ForReview")]
	    [HttpGet]
	    public ReviewPageViewModel Get(string relativeURL, ReviewPageViewModel reviewPageViewModel)
	    {
			if (string.IsNullOrWhiteSpace(relativeURL))
				throw new ArgumentNullException(nameof(relativeURL));

			return _commentService.GetCommentsAndQuestionsForReview(relativeURL, reviewPageViewModel);
	    }

	}
}
