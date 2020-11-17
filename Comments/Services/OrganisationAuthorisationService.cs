using Comments.Common;
using Comments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Comments.ViewModels;

namespace Comments.Services
{
	public interface IOrganisationAuthorisationService
	{
		OrganisationCode GenerateOrganisationCode(int organisationId, int consultationId);
	}

    public class OrganisationAuthorisationService : IOrganisationAuthorisationService
	{
        private readonly ConsultationsContext _context;
        private readonly IUserService _userService;

        public OrganisationAuthorisationService(ConsultationsContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
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
			bool collision; //it's _very_ unlikely the 12 digit random number generator will generate the same one twice, but it's possible. so let's try 10 times.
			const int maxTriesAtUnique = 10;
			var counter = 0;
			do
			{
				counter++;
				collationCode = GenerateCollationCode();
				collision = _context.CheckCollationCodeExists(collationCode);
			} while (collision && counter <= maxTriesAtUnique);
			
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
	        var firstPart = random.Next(100000, 999999);
	        var secondPart = random.Next(000000, 999999);
	        var collationCode = $"{firstPart:#### ##}{secondPart:## ####}";
	        return collationCode;
        }
	}
}
