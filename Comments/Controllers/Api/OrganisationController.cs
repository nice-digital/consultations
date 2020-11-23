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
    public class OrganisationController : ControllerBase
    {
	    private readonly IOrganisationService _organisationService;
        private readonly ILogger<OrganisationController> _logger;

        public OrganisationController(IOrganisationService organisationService, ILogger<OrganisationController> logger)
        {
	        _organisationService = organisationService;
	        _logger = logger;
        }

		/// <summary>
		/// This method is called by the front-end when the user clicks the generate code button, next to an organisation, on a consultation, on the download page.
		/// 
		/// POST: consultations/api/Organisation?organisationId=1&consultationId=1
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


			var collationCode = _organisationService.GenerateOrganisationCode(organisationId, consultationId);

            return Ok(collationCode);
        }

		/// <summary>
		/// This method is called when the user types in the collation/organisation code in the LoginBanner react component.
		/// It's called by ajax, potentially every 400msec, as the user types (there's some debouncing going on to limit it).
		///
		/// This method does not have the Authorize attribute as the user won't be logged in.
		///
		/// GET: consultations/api/Organisation?collationCode=123412341234&consultationId=1
		/// </summary>
		/// <param name="collationCode"></param>
		/// <param name="consultationId"></param>
		/// <returns></returns>
		[HttpGet("")]
		public async Task<IActionResult> CheckOrganisationCode(string collationCode, int consultationId)
		{
			ValidateCollationCode(collationCode);

			if (consultationId < 1)
				throw new ArgumentException("Consultation id must be a positive integer", nameof(consultationId));


			var organisationCode = await _organisationService.CheckValidCodeForConsultation(collationCode, consultationId);

			return Ok(organisationCode);
		}

		/// <summary>
		/// This action adds a record in the OrganisationUser table. It's used to create a session for an organisational commenter.
		/// It's called when the user clicks the confirm button when using an organisation/collation code on the LoginBanner react component.
		///
		/// This method does not have the Authorize attribute as the user won't be logged in.
		/// 
		/// POST: consultations/api/Organisation/CreateOrganisationUserSession?organisationAuthorisationId=1
		/// </summary>
		/// <param name="collationCode">collation code is required here for security purposes. the service does a lookup to check the collation code matches the OrganisationAuthorisationId</param>
		/// <param name="organisationAuthorisationId"></param>
		/// <returns>a GUID, which is the session id: OrganisationUser.AuthorisationSession</returns>
		[HttpPost("CreateOrganisationUserSession")]
		public IActionResult CreateOrganisationUserSession(string collationCode, int organisationAuthorisationId)
		{
			ValidateCollationCode(collationCode);

			if (organisationAuthorisationId < 1)
				throw new ArgumentException("OrganisationAuthorisation id must be a positive integer", nameof(organisationAuthorisationId));
			
			var authorisationSession = _organisationService.CreateOrganisationUserSession(organisationAuthorisationId, collationCode);

			return Ok(authorisationSession);
		}


		/// <summary>
		/// Helper method to validate collation code between different actions.
		/// </summary>
		/// <param name="collationCode"></param>
		private static void ValidateCollationCode(string collationCode)
		{
			if (string.IsNullOrEmpty(collationCode))
				throw new ArgumentException("Collation code cannot be null or empty string.", nameof(collationCode));

			var regex = new Regex(Constants.CollationCode.RegExSpacesRemoved);
			if (!regex.IsMatch(collationCode.Replace(" ", "")))
				throw new ArgumentException("Collation code is not correct format.");
		}
    }
}
