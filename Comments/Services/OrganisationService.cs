using Comments.Common;
using Comments.Models;
using Comments.ViewModels;
using NICE.Feeds;
using NICE.Identity.Authentication.Sdk.API;
using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authentication.Sdk.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Comments.Services
{
	public interface IOrganisationService
	{
		OrganisationCode GenerateOrganisationCode(int organisationId, int consultationId);
		Task<OrganisationCode> CheckValidCodeForConsultation(string collationCode, int consultationId);
		(Guid sessionId, DateTime expirationDate) CreateOrganisationUserSession(int organisationAuthorisationId, string collationCode);
		Task<(bool valid, string organisationName)> CheckOrganisationUserSession(int consultationId);
		IList<ValidatedSession> CheckValidCodesForConsultation(Session unvalidatedSessions);
		Task<Dictionary<int, string>> GetOrganisationNames(IEnumerable<int> organisationIds);
	}

    public class OrganisationService : IOrganisationService
	{
        private readonly ConsultationsContext _context;
        private readonly IUserService _userService;
        private readonly IApiTokenClient _apiTokenClient;
        private readonly IAPIService _apiService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConsultationService _consultationService;

        public OrganisationService(ConsultationsContext context, IUserService userService, IApiTokenClient apiTokenClient, IAPIService apiService, IHttpClientFactory httpClientFactory, IConsultationService consultationService)
        {
            _context = context;
            _userService = userService;
            _apiTokenClient = apiTokenClient;
            _apiService = apiService;
            _httpClientFactory = httpClientFactory;
            _consultationService = consultationService;
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
			var currentUsersOrganisationName = currentUser.OrganisationsAssignedAsLead.FirstOrDefault(org => org.OrganisationId.Equals(organisationId))?.OrganisationName;

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
			if (collision != null && counter >= maxTriesAtUnique)
				throw new ApplicationException("Couldn't generate a random number. Please contact app support.");
			
			//then save it to the db
			var organisationAuthorisation =_context.SaveCollationCode(sourceURI, currentUser.UserId, DateTime.UtcNow, organisationId, collationCode);

			return new OrganisationCode(organisationAuthorisation, currentUsersOrganisationName);
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
	        var firstPart = random.Next(100000, 999999); //preventing a leading zero, which a user might not type if they consider it a number.
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
			var genericCodeErrorMessage = "Incorrect code. Verify the code with your organisation's commenting lead and try again.";

			var organisationAuthorisation = _context.GetOrganisationAuthorisationByCollationCode(collationCode);
			if (organisationAuthorisation == null)
				throw new ApplicationException(genericCodeErrorMessage); //old message: "Collation code not found"

			var sourceURI = ConsultationsUri.CreateConsultationURI(consultationId);
			if (!organisationAuthorisation.Location.SourceURI.Equals(sourceURI, StringComparison.OrdinalIgnoreCase))
				throw new ApplicationException(genericCodeErrorMessage); //old message: "The supplied code is for a different consultation."

			var hasOrganisationSubmittedForThisConsultation = HasOrganisationSubmittedForConsultation(organisationAuthorisation.OrganisationId, sourceURI);
			if (hasOrganisationSubmittedForThisConsultation)
				throw new ApplicationException("Your organisation has already responded to this consultation. Contact your organisation's commenting lead for further information.");

			var organisationName = await GetOrganisationName(organisationAuthorisation.OrganisationId);

			return new OrganisationCode(organisationAuthorisation, organisationName);
		}

		//TODO: if performance is an issue, refactor this to be 1 DB hit instead of 2-4.
		private bool HasOrganisationSubmittedForConsultation(int organisationId, string sourceURI)
		{
			var submittedCommentParentIds = _context.GetAllSubmittedCommentsForURI(sourceURI).Where(comment => comment.ParentCommentId.HasValue).Select(comment => comment.ParentCommentId.Value).ToList();

			var anySubmittedCommentsForThisOrgnisation = _context.AreCommentsForThisOrganisation(submittedCommentParentIds, organisationId);
			if (anySubmittedCommentsForThisOrgnisation)
				return true;

			var submittedAnswerParentIds = _context.GetAllSubmittedAnswersForURI(sourceURI).Where(answer => answer.ParentAnswerId.HasValue).Select(answer => answer.ParentAnswerId.Value).ToList();
			return  _context.AreAnswersForThisOrganisation(submittedAnswerParentIds, organisationId);
		}


		public (Guid sessionId, DateTime expirationDate) CreateOrganisationUserSession(int organisationAuthorisationId, string collationCode)
		{
			var organisationAuthorisationForSuppliedCollationCode = _context.GetOrganisationAuthorisationByCollationCode(collationCode);
			if (organisationAuthorisationForSuppliedCollationCode == null || !organisationAuthorisationForSuppliedCollationCode.OrganisationAuthorisationId.Equals(organisationAuthorisationId))
				throw new ApplicationException($"Supplied collation code: {collationCode} is not valid for the supplied organisation authorisation id:{organisationAuthorisationId}");

			var consultationDetails = _consultationService.GetConsultationState(organisationAuthorisationForSuppliedCollationCode.Location.SourceURI, PreviewState.NonPreview);
			var expirationDate = consultationDetails.EndDate.AddDays(28);

			var organisationUser = _context.CreateOrganisationUser(organisationAuthorisationId, Guid.NewGuid(), expirationDate);
			return (organisationUser.AuthorisationSession, expirationDate);
		}

		public async Task<(bool valid, string organisationName)> CheckOrganisationUserSession(int consultationId) 
		{
			var currentUser = _userService.GetCurrentUser();
			var sessionId = currentUser.GetValidatedSessionIdForConsultation(consultationId);

			if (!sessionId.HasValue)
			{
				return (valid: false, organisationName: null);
			}

			var organisationUser = _context.GetOrganisationUsers(new List<Guid> {sessionId.Value}).FirstOrDefault();

			if (organisationUser == null)
			{
				return (valid: false, organisationName: null);
			}

			var organisationName = await GetOrganisationName(organisationUser.OrganisationAuthorisation.OrganisationId);
			return (valid: true, organisationName: organisationName);
		}

		public IList<ValidatedSession> CheckValidCodesForConsultation(Session unvalidatedSessions)
		{
			var organisationUsers = _context.GetOrganisationUsers(unvalidatedSessions.SessionCookies.Select(session => session.Value)).ToList();

			return unvalidatedSessions.SessionCookies.Select(session =>
			{
				var organisationUserForThisSession = organisationUsers.FirstOrDefault(ou => ou.AuthorisationSession.Equals(session.Value));
				if (organisationUserForThisSession != null && organisationUserForThisSession.ExpirationDate > DateTime.UtcNow)
				{
					var parsedUri = ConsultationsUri.ParseConsultationsUri(organisationUserForThisSession.OrganisationAuthorisation.Location.SourceURI);
					if (parsedUri.ConsultationId.Equals(session.Key))
					{
						var organisationUserId = organisationUserForThisSession.OrganisationUserId;
						var organisationId = organisationUserForThisSession.OrganisationAuthorisation.OrganisationId;
						return new ValidatedSession(organisationUserId, session.Key, session.Value, organisationId);
					}
				}
				return null;
			}).Where(session => session != null).ToList();
		}

		private async Task<string> GetOrganisationName(int organisationId)
		{
			var organisationIdAndNames = await GetOrganisationNames(new List<int> {organisationId});

			if (!organisationIdAndNames.ContainsKey(organisationId))
				throw new ApplicationException("Organisation name could not be retrieved. Please contact app support."); //might occur if the org has been deleted from idam and CC hasn't been updated.

			return organisationIdAndNames[organisationId];

		}

		public async Task<Dictionary<int, string>> GetOrganisationNames(IEnumerable<int> organisationIds)
		{
			var cachedM2MAccessToken = new JwtToken { AccessToken = await _apiTokenClient.GetAccessToken()};

			var httpClientWithPooledMessageHandler = _httpClientFactory.CreateClient();

			var organisations = await _apiService.GetOrganisations(organisationIds.Distinct(), cachedM2MAccessToken, httpClientWithPooledMessageHandler);

			return organisations.ToDictionary(k => k.OrganisationId, v => v.OrganisationName);
		}
	}
}
