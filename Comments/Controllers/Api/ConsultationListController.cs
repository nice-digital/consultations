using Comments.Services;
using Comments.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Comments.Controllers.Api
{
	[Produces("application/json")]
    [Route("consultations/api/[controller]")]
	//[Authorize(Roles = "Administrator")] - authorisation is now in the constructor of the service
    public class ConsultationListController : Controller
    {
        private readonly IConsultationListService _consultationListService;
        private readonly ILogger<ConsultationListController> _logger;

        public ConsultationListController(IConsultationListService consultationListService, ILogger<ConsultationListController> logger)
        {
	        _consultationListService = consultationListService;
            _logger = logger;
        }

		/// <summary>
		/// GET: eg. consultations/api/ConsultationList
		/// </summary>
		/// <returns></returns>
		[HttpGet]
        public ConsultationListViewModel Get(ConsultationListViewModel consultationListViewModel)
		{
			return _consultationListService.GetConsultationListViewModel(consultationListViewModel);
		}
    }
}
