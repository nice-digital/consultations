using Comments.Common;
using Comments.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Comments.Services
{
	public interface IOrganisationAuthorisationService
	{
		string GenerateOrganisationCode(int organisationId, int consultationId);
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

        public string GenerateOrganisationCode(int organisationId, int consultationId)
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
			bool collision; //it's very unlikely the 12 digit random number generator will generate the same one twice, but it's possible. so try 10 times.
			const int maxTriesAtUnique = 10;
			var counter = 0;
			do
			{
				counter++;
				collationCode = GenerateCollationCode(organisationId, consultationId);
				collision = _context.CheckCollationCodeExists(collationCode);
			} while (collision && counter <= maxTriesAtUnique);
			

			//then save it to the db
			//TODO

			return collationCode;
        }

        private string GenerateCollationCode(int organisationId, int consultationId)
        {
	        var random = new Random();
	        var firstPart = random.Next(100000, 999999);
	        var secondPart = random.Next(000000, 999999);
	        var collationCode = $"{firstPart:#### ##}{secondPart:## ####}";
	        return collationCode;
        }
	}
}
