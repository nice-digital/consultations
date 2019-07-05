using Comments.Services;
using Comments.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Comments.Controllers.Api
{
	[Authorize(Roles = "Administrator,CommentAdminTeam,IndevUser")]
	[Produces("application/json")]
    [Route("consultations/api/[controller]")]
    public class AnalysisController : Controller
    {
		private readonly ICommentService _commentService;
		private readonly ILogger<CommentsController> _logger;

		public AnalysisController(ICommentService commentService, ILogger<CommentsController> logger)
		{
			_commentService = commentService;
			_logger = logger;
		}

		/// <summary>
		/// GET: eg. consultations/api/Analysis?consultationId=1
		/// </summary>
		/// <param name="consultationId"></param>
		/// <returns></returns>
		[HttpGet]
		public CommentsAndQuestionsWithAnalysis Get(int consultationId)
		{
			if (consultationId < 1)
				throw new ArgumentException(nameof(consultationId));

			return _commentService.GetCommentsAndQuestionsWithAnalysis(consultationId);
		}
	}
}
