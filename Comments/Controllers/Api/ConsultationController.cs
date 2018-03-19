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
    public class ConsultationController : Controller
    {
        private readonly IConsultationService _consultationService;
        private readonly ILogger<ConsultationController> _logger;

        public ConsultationController(IConsultationService consultationService, ILogger<ConsultationController> logger)
        {
            _consultationService = consultationService;
            _logger = logger;
        }

        /// <summary>
        /// GET: eg. consultations/api/Consultation?consultationId=1
        /// </summary>
        /// <param name="consultationId"></param>
        /// <returns></returns>
        [HttpGet]
        public ViewModels.Consultation Get(int consultationId)
        {
            if (consultationId < 1)
                throw new ArgumentException(nameof(consultationId));

            return _consultationService.GetConsultation(consultationId);
        }
    }
}
