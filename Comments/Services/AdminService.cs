using Comments.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using Microsoft.AspNetCore.Hosting;

namespace Comments.Services
{
	public interface IAdminService
	{
		int DeleteAllData();
		int DeleteAllSubmissionsFromUser(Guid usersSubmissionsToDelete);
		int InsertQuestionsForDocument1And2InConsultation(int consultationId);
		int InsertQuestionsForConsultation(int consultationId);
		int InsertQuestionsForCfGConsultation(int consultationId);
		IList<object> GetData(string tableName);
	}

	/// <summary>
	/// This functionality is for admins only and is secured by an Authorize attribute on the controller AND by a role check in the constructor of this class.
	/// </summary>
    public class AdminService : IAdminService
    {
	    private readonly ConsultationsContext _dbContext;
	    private readonly IHostingEnvironment _hostingEnvironment;

	    public AdminService(IUserService userService, IHttpContextAccessor httpContextAccessor, ConsultationsContext dbContext, IHostingEnvironment hostingEnvironment)
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
		    _hostingEnvironment = hostingEnvironment;
	    }

	    public int DeleteAllData()
	    {
		    if (_hostingEnvironment.IsProduction())
		    {
			    throw new Exception("Not allowed to do this on production");
		    }
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

	    public int InsertQuestionsForCfGConsultation(int consultationId)
	    {
		    return _dbContext.InsertQuestionsWithScriptForCfGConsultation(consultationId);
	    }

		public IList<object> GetData(string tableName)
	    {
		    return _dbContext.GetAllOfATable(tableName);
	    }
	}
}
