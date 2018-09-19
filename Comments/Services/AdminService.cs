using System;
using Comments.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Authentication;

namespace Comments.Services
{
	public interface IAdminService
	{
		int DeleteAllData();
		int DeleteAllSubmissionsFromUser(Guid usersSubmissionsToDelete);
		int InsertQuestionsForDocument1And2InConsultation(int consultationId);
		int InsertQuestionsForConsultation(int consultationId);
	}

	/// <summary>
	/// This functionality is for admins only and is secured by an Authorize attribute on the controller AND by a role check in the constructor of this class.
	/// </summary>
    public class AdminService : IAdminService
    {
	    private readonly ConsultationsContext _dbContext;

	    public AdminService(IUserService userService, IHttpContextAccessor httpContextAccessor, ConsultationsContext dbContext)
	    {
		    var user = userService.GetCurrentUser();
		    if (!user.IsAuthorised)
		    {
				throw new AuthenticationException("GetCurrentUser returned null");
		    }
		    var niceUser = httpContextAccessor.HttpContext.User;
		    if (!niceUser.Identity.IsAuthenticated)
		    {
			    throw new AuthenticationException("NICE user is not authenticated");
			}
			if (!niceUser.IsInRole("Administrator"))
			{
			    throw new AuthenticationException("NICE user is not an administrator");
		    }
		    _dbContext = dbContext;
		}

	    public int DeleteAllData()
	    {
		    return _dbContext.DeleteEverything();
	    }

	    public int DeleteAllSubmissionsFromUser(Guid usersSubmissionsToDelete)
	    {
		    return _dbContext.DeleteAllSubmissionsFromUser(usersSubmissionsToDelete);
	    }

	    public int InsertQuestionsForDocument1And2InConsultation(int consultationId)
	    {
		    return _dbContext.InsertQuestionsWithScriptForDocument1And2InConsultation(consultationId);
	    }

	    public int InsertQuestionsForConsultation(int consultationId)
	    {
		    return _dbContext.InsertQuestionsWithScriptForConsultation(consultationId);
	    }
	}
}
