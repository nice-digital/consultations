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
    public class DocumentsController : Controller
    {
        private readonly IConsultationService _consultationService;
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(IConsultationService consultationService, ILogger<DocumentsController> logger)
        {
            _consultationService = consultationService;
            _logger = logger;
		}

		// GET: eg. consultations/api/Documents?consultationId=1
		/// <summary>
		/// Get all documents contained within a specified consultation
		/// </summary>
		/// <param name="consultationId">Id of the consultation</param>
		/// <returns>IEnumerable of Document</returns>
		[HttpGet]
		[ProducesResponseType(typeof(IEnumerable<Document>), StatusCodes.Status200OK)]
		public IEnumerable<Document> Get(int consultationId)
	    {
		    if (consultationId < 1)
			    throw new ArgumentException(nameof(consultationId));

		    return _consultationService.GetDocuments(consultationId).documents;
	    }
	}

	[Produces("application/json")]
	[Route("consultations/api/[controller]")]
	[Authorize(Roles="IndevUser")]
	public class PreviewDraftDocumentsController : Controller
	{
		private readonly IConsultationService _consultationService;
		private readonly ILogger<DocumentsController> _logger;

		public PreviewDraftDocumentsController(IConsultationService consultationService, ILogger<DocumentsController> logger)
		{
			_consultationService = consultationService;
			_logger = logger;
		}

		// GET: eg. consultations/api/PreviewDocuments?consultationId=1&documentId=1&reference=GID-TA1232
		/// <summary>
		/// Returns preview documents contained within a specified consultation.
		/// </summary>
		/// <param name="consultationId">Id of the consultation</param>
		/// <param name="documentId"> Id of the document</param>
		/// <param name="reference">Reference Id of the consultation e.g. GID-TA1232</param>
		/// <returns>IEnumerable of Document</returns>
		[HttpGet]
		[ProducesResponseType(typeof(IEnumerable<Document>), StatusCodes.Status200OK)]
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
	[Authorize(Roles="IndevUser")]
	public class PreviewPublishedDocumentsController : Controller
	{
		private readonly IConsultationService _consultationService;
		private readonly ILogger<DocumentsController> _logger;

		public PreviewPublishedDocumentsController(IConsultationService consultationService, ILogger<DocumentsController> logger)
		{
			_consultationService = consultationService;
			_logger = logger;
		}

		// GET: eg. consultations/api/PreviewDocuments?consultationId=1&documentId=1&reference=GID-TA1232
		/// <summary>
		/// Returns published documents contained within a specified consultation.
		/// </summary>
		/// <param name="consultationId">Id of the consultation</param>
		/// <param name="documentId"> Id of the document</param>
		/// <returns>IEnumerable of Document</returns>
		[HttpGet]
		[ProducesResponseType(typeof(IEnumerable<Document>), StatusCodes.Status200OK)]
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
