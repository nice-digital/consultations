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
        /// GET: eg. consultations/api/Comments?sourceURI=http%3A%2F%2Fwww.nice.org.uk%2Fconsultations%2F1%2F1%2Fchapter-slug
        /// </summary>
        /// <param name="sourceURI"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<Document> Get(int consultationId)
        {
            if (consultationId < 1)
                throw new ArgumentException(nameof(consultationId));

            return _consultationService.GetDocuments(consultationId);
        }
    }
}
