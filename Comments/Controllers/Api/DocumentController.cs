using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comments.Models;
using Comments.Services;
using Comments.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Comments.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Document")]
    public class DocumentController : Controller
    {
        private readonly IConsultationService _consultationService;
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(IConsultationService consultationService, ILogger<DocumentController> logger)
        {
            _consultationService = consultationService;
            _logger = logger;
        }

        // GET: eg. api/Document?consultationId=1&documentId=1
        [HttpGet]
        public DocumentViewModel Get(int consultationId, int documentId)
        {
            return _consultationService.GetAllCommentsAndQuestionsForDocument(consultationId, documentId);
        }

        //// GET: api/Document/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}
        
        //// POST: api/Document
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}
        
        //// PUT: api/Document/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}
        
        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
