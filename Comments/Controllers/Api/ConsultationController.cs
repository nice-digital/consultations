using Comments.Services;
using Comments.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Comments.Controllers.Api
{
    [Produces("application/json")]
    [Route("consultations/api/[controller]")]
    public class ConsultationController : Controller
    {
        private readonly IConsultationService _consultationService;
        private readonly ILogger<ConsultationController> _logger;

        public ConsultationController(IConsultationService consultationService, ILogger<ConsultationController> logger)
        {
            _consultationService = consultationService;
            _logger = logger;
		}

        // GET: eg. consultations/api/Consultation?consultationId=1
		/// <summary>
		/// Returns details of a single consultation
		/// </summary>
		/// <param name="consultationId"> Id of the consultation</param>
		/// <param name="isReview">Boolean indicating if the feed isbeing accessed for reviewing purposes</param>
		/// <returns>Consultation ViewModel</returns>
		[HttpGet]
        public ViewModels.Consultation Get(int consultationId, bool isReview = false)
        {
            if (consultationId < 1)
                throw new ArgumentException(nameof(consultationId));

            return _consultationService.GetConsultation(consultationId, (isReview ? BreadcrumbType.Review : BreadcrumbType.DocumentPage), useFilters: isReview);
        }
    }

	[Produces("application/json")]
	[Route("consultations/api/[controller]")]
	[Authorize(Roles="IndevUser")]
	public class DraftConsultationController : Controller
	{
		private readonly IConsultationService _consultationService;
		private readonly ILogger<ConsultationController> _logger;

		public DraftConsultationController(IConsultationService consultationService, ILogger<ConsultationController> logger)
		{
			_consultationService = consultationService;
			_logger = logger;
		}

		// GET: eg. consultations/api/DraftConsultation?consultationId=1&documentId=1&reference=GID-TA1232
		/// <summary>
		/// Returns details for a single consultation that has never been published.
		/// </summary>
		/// <param name="consultationId">Id of the consultation</param>
		/// <param name="documentId"> Id of the document</param>
		/// <param name="reference">Reference Id of the consultation e.g. GID-TA1232</param>
		/// <param name="isReview">Boolean indicating if the feed is being accessed for reviewing purposes</param>
		/// <returns>Consultation ViewModel</returns>
		[HttpGet]
		[ProducesResponseType(typeof(Consultation), StatusCodes.Status200OK)]
		public ViewModels.Consultation Get(int consultationId, int documentId, string reference, bool isReview = false)
		{
			if (consultationId < 1)
				throw new ArgumentException(nameof(consultationId));

			return _consultationService.GetDraftConsultation(consultationId, documentId, reference, isReview);
		}
	}
}
