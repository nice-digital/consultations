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
	    /// GET: eg. consultations/api/Review/1
	    /// </summary>
	    /// <param name="consultationId">id of the consultation eg "1"</param>
	    /// <returns></returns>
	    [HttpGet("{consultationId}")]
	    public CommentsAndQuestions Get([FromRoute] int consultationId)
        {
            if (consultationId < 1)
                throw new ArgumentNullException(nameof(consultationId));

            return _commentService.GetUsersCommentsAndQuestionsForConsultation(consultationId);
        }
    }
}
