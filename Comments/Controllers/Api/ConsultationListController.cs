using Comments.Services;
using Comments.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Comments.Controllers.Api
{
	[Produces("application/json")]
    [Route("consultations/api/[controller]")]
	//[Authorize(Roles = "Administrator")] - authorisation is in the service as the role list is configurable in appsettings.json
    public class ConsultationListController : ControllerBase
	{
        private readonly IConsultationListService _consultationListService;
        private readonly ILogger<ConsultationListController> _logger;
	    private readonly ISecurityService _securityService;

	    public ConsultationListController(IConsultationListService consultationListService, ILogger<ConsultationListController> logger, ISecurityService securityService)
        {
	        _consultationListService = consultationListService;
            _logger = logger;
	        _securityService = securityService;
        }

		/// <summary>
		/// GET: eg. consultations/api/ConsultationList
		/// </summary>
		/// <returns></returns>
		[HttpGet]
        public IActionResult Get(ConsultationListViewModel consultationListViewModel)
		{
			var result = _consultationListService.GetConsultationListViewModel(consultationListViewModel);

			var invalidResult = Validate(result.validate, _logger);

			return invalidResult ?? Ok(result.consultationListViewModel);
		}
    }
}
