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

        // POST: consultations/api/OrganisationAuthorisation
		[HttpPost("")]
        public IActionResult Post(int organisationId, int consultationId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var collationCode = _organisationAuthorisationService.GenerateOrganisationCode(organisationId, consultationId);

            return Ok(collationCode);
        }
    }
}
