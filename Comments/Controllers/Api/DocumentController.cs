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
        private readonly ICommentService _commentService;
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(ICommentService commentService, ILogger<DocumentController> logger)
        {
            _commentService = commentService;
            _logger = logger;
        }

        /// <summary>
        /// GET: eg. api/Document?consultationId=1&documentId=1&chapterSlug=chapter-slug
        /// </summary>
        /// <param name="consultationId"></param>
        /// <param name="documentId">nullable. if null, show first commentable document.</param>
        /// <param name="chapterSlug">nullable. if not, show first chapter.</param>
        /// <returns></returns>
        [HttpGet]
        public DocumentViewModel Get(int consultationId, int? documentId, string chapterSlug)
        {
            return _commentService.GetAllCommentsAndQuestionsForDocument(consultationId, documentId, chapterSlug);
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
