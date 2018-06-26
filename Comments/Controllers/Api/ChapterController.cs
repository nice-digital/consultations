using Comments.Services;
using Comments.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Comments.Controllers.Api
{
    [Produces("application/json")]
    [Route("consultations/api/[controller]")]
    public class ChapterController : Controller
    {
        private readonly IConsultationService _consultationService;
        private readonly ILogger<ChapterController> _logger;

        public ChapterController(IConsultationService consultationService, ILogger<ChapterController> logger)
        {
            _consultationService = consultationService;
            _logger = logger;
        }

		/// <summary>
		/// GET: eg. consultations/api/Chapter?consultationId=1&documentId=2&chapterSlug=introduction or consultations/api/Chapter?consultationId=1&documentId=2&chapterSlug=introduction&reference=GID-TA10232
		/// </summary>
		/// <param name="consultationId">int</param>
		/// <param name="documentId">int</param>
		/// <param name="chapterSlug">non-empty string</param>
		/// <param name="reference">non-empty string</param>
		/// <returns></returns>
		[HttpGet]
        public ChapterContent Get(int consultationId, int documentId, string chapterSlug, string reference = null)
        {
            if (string.IsNullOrWhiteSpace(chapterSlug))
                throw new ArgumentNullException(nameof(chapterSlug));

	        if (string.IsNullOrWhiteSpace(reference))
				return _consultationService.GetChapterContent(consultationId, documentId, chapterSlug);

	        return _consultationService.GetPreviewChapterContent(consultationId, documentId, chapterSlug, reference);
		}
	}
}
