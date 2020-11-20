using System;
using System.Data;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Comments.Common;
using Comments.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Comments.Controllers.Api
{
	[Produces("application/json")]
    [Route("consultations/api/[controller]")]
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
		[Authorize]
		public IActionResult GenerateCode(int organisationId, int consultationId)
        {
	        if (organisationId < 1)
		        throw new ArgumentException("Organisation id must be a positive integer", nameof(organisationId));
			if (consultationId < 1)
				throw new ArgumentException("Consultation id must be a positive integer", nameof(consultationId));


			var collationCode = _organisationAuthorisationService.GenerateOrganisationCode(organisationId, consultationId);

            return Ok(collationCode);
        }

		/// <summary>
		/// This method is called when the user types in the collation/organisation code in the LoginBanner react component.
		/// It's called by ajax, potentially every 400msec, as the user types (there's some debouncing going on to limit it).
		///
		/// This method does not have the Authorize attribute as the user won't be logged in.
		///
		/// GET: consultations/api/OrganisationAuthorisation?collationCode=123412341234&consultationId=1
		/// </summary>
		/// <param name="collationCode"></param>
		/// <param name="consultationId"></param>
		/// <returns></returns>
		[HttpGet("")]
		public async Task<IActionResult> CheckOrganisationCode(string collationCode, int consultationId)
		{
			if (consultationId < 1)
				throw new ArgumentException("Consultation id must be a positive integer", nameof(consultationId));

			if (string.IsNullOrEmpty(collationCode))
				throw new ArgumentException("Collation code cannot be null or empty string.", nameof(collationCode));

			var regex = new Regex(Constants.CollationCode.RegExSpacesRemoved);
			if (!regex.IsMatch(collationCode.Replace(" ", "")))
				throw new ArgumentException("Collation code is not correct format.");
			
			try
			{
				var organisationCode = await _organisationAuthorisationService.CheckValidCodeForConsultation(collationCode, consultationId);

				 return Ok(organisationCode);
			}
			catch (ApplicationException ae)
			{
				return NotFound(ae.Message);
			}
			catch (AccessViolationException ave)
			{
				return Forbid(ave.Message);
			}
			catch (DataException de)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError, de.Message);
			}
		}
	}
}
