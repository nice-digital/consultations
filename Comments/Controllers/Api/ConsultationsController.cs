using Comments.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Comments.ViewModels;
using Microsoft.AspNetCore.Http;

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

        // GET: eg. consultations/api/Consultations
        /// <summary>
        /// Returns a list of all published consultations
        /// </summary>
        /// <returns>IEnumerable of ViewModel.Consultation</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Consultation>), StatusCodes.Status200OK)]
		public IEnumerable<ViewModels.Consultation> Get()
        {
            return _consultationService.GetConsultations();
        }
    }
}
