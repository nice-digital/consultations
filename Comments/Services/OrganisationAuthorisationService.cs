using System;
using System.Collections.Generic;
using System.Linq;
using Comments.Common;
using Comments.Models;
using Comments.ViewModels;
using Microsoft.AspNetCore.Http;

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
	    private readonly IHttpContextAccessor _httpContextAccessor;
	    private readonly User _currentUser;

        public OrganisationAuthorisationService(ConsultationsContext context, IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userService = userService;
	        _httpContextAccessor = httpContextAccessor;
	        _currentUser = _userService.GetCurrentUser();
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
			var collationCode = GenerateCollationCode(organisationId, consultationId);
			

			//then save it to the db

			return collationCode;
        }

        public string GenerateCollationCode(int organisationId, int consultationId)
        {
	        return "TODO: generate a real code";
        }
	}
}
