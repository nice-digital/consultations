using Comments.Services;
using Comments.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Comments.Controllers.Api
{
	[Produces("application/json")]
    [Route("consultations/api/[controller]")]
	//[Authorize(Policy = "Administrator")] - authorisation is in the service as the role list is configurable in appsettings.json
	public class ConsultationListController : ControllerBase
	{
        private readonly IConsultationListService _consultationListService;
        private readonly ILogger<ConsultationListController> _logger;

	    public ConsultationListController(IConsultationListService consultationListService, ILogger<ConsultationListController> logger)
        {
	        _consultationListService = consultationListService;
            _logger = logger;
        }

		/// <summary>
		/// GET: eg. consultations/api/ConsultationList?Status=Open&Status=Closed
		/// note: the repeated querystring parameter. this is how default model binding to an array works. don't pass a CSV list and expect it to work without writing something custom (like I did)
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
