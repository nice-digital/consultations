using Comments.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Comments.Controllers.Api
{
	[Produces("application/json")]
    [Route("consultations/api/[controller]")]
    [Authorize]
    public class OrganisationAuthorisationController : ControllerBase
    {
	    private readonly IOrganisationAuthorisationService _organisationAuthorisationService;
        private readonly ILogger<OrganisationAuthorisationController> _logger;

        public OrganisationAuthorisationController(IOrganisationAuthorisationService organisationAuthorisationService, ILogger<OrganisationAuthorisationController> logger)
        {
	        _organisationAuthorisationService = organisationAuthorisationService;
	        _logger = logger;
        }

		/// <summary>
		/// This method is called by the front-end when the user clicks the generate code button, next to an organisation, on a consultation, on the download page.
		/// 
		/// POST: consultations/api/OrganisationAuthorisation?organisationId=1&consultationId=1
		/// </summary>
		/// <param name="organisationId"></param>
		/// <param name="consultationId"></param>
		/// <returns></returns>
		[HttpPost("")]
        public IActionResult GenerateCode(int organisationId, int consultationId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var collationCode = _organisationAuthorisationService.GenerateOrganisationCode(organisationId, consultationId);

            return Ok(collationCode);
        }

		/// <summary>
		/// This method is called when the user types in the collation/organisation code in the LoginBanner react component.
		/// It's called by ajax, potentially every 400msec, as the user types (there's some debouncing going on to limit it).
		///
		/// GET: consultations/api/OrganisationAuthorisation?collationCode=123412341234&consultationId=1
		/// </summary>
		/// <param name="collationCode"></param>
		/// <param name="consultationId"></param>
		/// <returns></returns>
		[HttpGet("")]
		public IActionResult CheckOrganisationCode(string collationCode, int consultationId)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var organisationCode = _organisationAuthorisationService.CheckValidCodeForConsultation(collationCode, consultationId);

			return Ok(organisationCode);
		}
	}
}
