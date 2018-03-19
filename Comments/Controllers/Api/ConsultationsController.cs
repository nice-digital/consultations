using Comments.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Comments.Controllers.Api
{
    [Produces("application/json")]
    [Route("consultations/api/[controller]")]
    public class ConsultationsController : Controller
    {
        private readonly IConsultationService _consultationService;
        private readonly ILogger<ConsultationController> _logger;

        public ConsultationsController(IConsultationService consultationService, ILogger<ConsultationController> logger)
        {
            _consultationService = consultationService;
            _logger = logger;
        }

        /// <summary>
        /// GET: eg. consultations/api/Consultations
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<ViewModels.Consultation> Get()
        {
            return _consultationService.GetConsultations();
        }
    }
}