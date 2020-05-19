using Comments.Services;
using Comments.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Comments.Controllers.Api
{
    [Produces("application/json")]
    [Route("consultations/api/[controller]")]
    public class DocumentsController : Controller
    {
        private readonly IConsultationService _consultationService;
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(IConsultationService consultationService, ILogger<DocumentsController> logger)
        {
            _consultationService = consultationService;
            _logger = logger;
        }

	    /// <summary>
	    /// GET: eg. consultations/api/Documents?consultationId=1
	    /// </summary>
	    /// <param name="consultationId"></param>
	    /// <returns></returns>
	    [HttpGet]
	    public IEnumerable<Document> Get(int consultationId)
	    {
		    if (consultationId < 1)
			    throw new ArgumentException(nameof(consultationId));

		    return _consultationService.GetDocuments(consultationId).documents;
	    }
	}

	[Produces("application/json")]
	[Route("consultations/api/[controller]")]
	[Authorize(Policy = "Administrator,CommentAdminTeam,IndevUser")]
	public class PreviewDraftDocumentsController : Controller
	{
		private readonly IConsultationService _consultationService;
		private readonly ILogger<DocumentsController> _logger;

		public PreviewDraftDocumentsController(IConsultationService consultationService, ILogger<DocumentsController> logger)
		{
			_consultationService = consultationService;
			_logger = logger;
		}

		/// <summary>
		/// GET: eg. consultations/api/PreviewDocuments?consultationId=1&documentId=1&reference=GID-TA1232
		/// </summary>
		/// <param name="consultationId"></param>
		/// <param name="documentId"></param>
		/// <param name="reference"></param>
		/// <returns></returns>
		[HttpGet]
		public IEnumerable<Document> Get(int consultationId, int documentId, string reference)
		{
			if (consultationId < 1)
				throw new ArgumentException(nameof(consultationId));

			if (documentId < 1)
				throw new ArgumentException(nameof(documentId));

			if (string.IsNullOrWhiteSpace(reference))
				throw new ArgumentNullException(nameof(reference));

			return _consultationService.GetPreviewDraftDocuments(consultationId, documentId, reference);
		}
	}

	[Produces("application/json")]
	[Route("consultations/api/[controller]")]
	[Authorize(Policy = "Administrator,CommentAdminTeam,IndevUser")]
	public class PreviewPublishedDocumentsController : Controller
	{
		private readonly IConsultationService _consultationService;
		private readonly ILogger<DocumentsController> _logger;

		public PreviewPublishedDocumentsController(IConsultationService consultationService, ILogger<DocumentsController> logger)
		{
			_consultationService = consultationService;
			_logger = logger;
		}

		/// <summary>
		/// GET: eg. consultations/api/PreviewDocuments?consultationId=1&documentId=1&reference=GID-TA1232
		/// </summary>
		/// <param name="consultationId"></param>
		/// <param name="documentId"></param>
		/// <param name="reference"></param>
		/// <returns></returns>
		[HttpGet]
		public IEnumerable<Document> Get(int consultationId, int documentId)
		{
			if (consultationId < 1)
				throw new ArgumentException(nameof(consultationId));

			if (documentId < 1)
				throw new ArgumentException(nameof(documentId));
			

			return _consultationService.GetPreviewPublishedDocuments(consultationId, documentId);
		}
	}
}
