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
			//first we need to check the current user is an organisation lead.
			
			//then check to see if the organisation already has an collation code for this consultation.
			
			//then generate a new code. ensure it's unique and valid according to some rules.

			return "TODO: generate a real code";
        }
	}
}
