using Comments.Common;
using Comments.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Comments.Configuration;
using Comments.ViewModels;
using NICE.Identity.Authentication.Sdk.API;
using NICE.Identity.Authentication.Sdk.Authorisation;

namespace Comments.Services
{
	public interface IOrganisationAuthorisationService
	{
		OrganisationCode GenerateOrganisationCode(int organisationId, int consultationId);
		Task<OrganisationCode> CheckValidCodeForConsultation(string collationCode, int consultationId);
	}

    public class OrganisationAuthorisationService : IOrganisationAuthorisationService
	{
        private readonly ConsultationsContext _context;
        private readonly IUserService _userService;
        private readonly IApiToken _apiTokenService;
        private readonly IAPIService _apiService;
        private readonly IHttpClientFactory _httpClientFactory;
		
        public OrganisationAuthorisationService(ConsultationsContext context, IUserService userService, IApiToken apiTokenService, IAPIService apiService, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _userService = userService;
            _apiTokenService = apiTokenService;
            _apiService = apiService;
            _httpClientFactory = httpClientFactory;
        }

		/// <summary>
		/// This function determines whether the current user can generate an OrganisationAuthorisation entry in the database, for the supplied organisation id and consultation id.
		/// It then generates the code using a random number generator, and confirms it doesn't already exist. 
		/// </summary>
		/// <param name="organisationId"></param>
		/// <param name="consultationId"></param>
		/// <returns></returns>
        public OrganisationCode GenerateOrganisationCode(int organisationId, int consultationId)
        {
			//first we need to check the current user is an organisation lead of the organisation id passed in.
			var currentUser = _userService.GetCurrentUser();
			if (currentUser.OrganisationsAssignedAsLead == null ||
			    !currentUser.OrganisationsAssignedAsLead.Any(org => org.OrganisationId.Equals(organisationId) && org.IsLead))
			{
				throw new UnauthorizedAccessException($"User: {currentUser.UserId} is not a lead of the organisation with id: {organisationId}");
			}

			//then check to see if the organisation already has an collation code for this consultation.
			var sourceURI = ConsultationsUri.CreateConsultationURI(consultationId);
			var existingOrganisationAuthorisation = _context.GetOrganisationAuthorisations(new List<string> {sourceURI});
			if (existingOrganisationAuthorisation.Any(oa => oa.OrganisationId.Equals(organisationId))) 
			{
				throw new ApplicationException($"There is already a collation code for consultation: {consultationId} and organisation: {organisationId}");
			}

			//then generate a new code. ensure it's unique and valid according to some rules.
			string collationCode;
			OrganisationAuthorisation collision; //it's _very_ unlikely the 12 digit random number generator will generate the same one twice, but it's possible. so let's try 10 times.
			const int maxTriesAtUnique = 10;
			var counter = 0;
			do
			{
				counter++;
				collationCode = GenerateCollationCode();
				collision = _context.GetOrganisationAuthorisationByCollationCode(collationCode);
			} while (collision != null && counter <= maxTriesAtUnique);
			
			//then save it to the db
			var organisationAuthorisation =_context.SaveCollationCode(sourceURI, currentUser.UserId, DateTime.UtcNow, organisationId, collationCode);

			return new OrganisationCode(organisationAuthorisation, currentUser.OrganisationsAssignedAsLead.FirstOrDefault(org => org.OrganisationId.Equals(organisationId))?.OrganisationName);
        }

		/// <summary>
		/// this outputs a collation code like this: "[4 numbers][space][4 numbers][space][4 numbers]"
		///
		/// the spaces are stripped before saving in the database, as the database column is nvarchar(12)
		/// </summary>
		/// <returns></returns>
		private string GenerateCollationCode()
        {
	        var random = new Random();
	        var firstPart = random.Next(100000, 999999); //preventing a leading zero, which a user might not type, if they consider it a number.
	        var secondPart = random.Next(0, 999999);
	        var collationCode = $"{firstPart:0000 00}{secondPart:00 0000}";
	        return collationCode;
        }

		/// <summary>
		/// Checks a collation code is valid for a given consultation
		/// </summary>
		/// <param name="collationCode"></param>
		/// <param name="consultationId"></param>
		/// <returns>Returns null if the collation code is not valid</returns>
		public async Task<OrganisationCode> CheckValidCodeForConsultation(string collationCode, int consultationId)
		{
			var organisationAuthorisation = _context.GetOrganisationAuthorisationByCollationCode(collationCode);
			if (organisationAuthorisation == null)
				throw new ApplicationException("Collation code not found");

			var sourceURI = ConsultationsUri.CreateConsultationURI(consultationId);
			if (!organisationAuthorisation.Location.SourceURI.Equals(sourceURI, StringComparison.OrdinalIgnoreCase))
				throw new AccessViolationException("The supplied collation code is for a different consultation.");


			try
			{
				var machineToMachineAccessToken =
					await _apiTokenService.GetAccessToken(AppSettings.AuthenticationConfig
						.GetAuthConfiguration()); //TODO: this needs caching!!!!!
				var httpClientWithPooledMessageHandler = _httpClientFactory.CreateClient();

				var organisations = await _apiService.GetOrganisations(
					new List<int> {organisationAuthorisation.OrganisationId},
					machineToMachineAccessToken,
					httpClientWithPooledMessageHandler);

				var organisation = organisations.FirstOrDefault();
				if (organisation == null)
					throw new DataException("Organisation name could not be retrieved.");

				return new OrganisationCode(organisationAuthorisation, organisation.OrganisationName);
			}
			catch (Exception ex)
			{

				throw;
			}

		}
	}
}
