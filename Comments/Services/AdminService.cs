using Comments.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;

namespace Comments.Services
{
	public interface IAdminService
	{
		//int DeleteAllData();
		int DeleteAllSubmissionsFromUser(string usersSubmissionsToDelete);
		int InsertQuestionsForDocument1And2InConsultation(int consultationId);
		int InsertQuestionsForConsultation(int consultationId);
		int InsertQuestionsForCfGConsultation(int consultationId);
		int InsertQuestionsForQSConsultation(int consultationId);
		IList<object> GetData(string tableName);
		IEnumerable<string> GetUniqueUsers();
	}

	/// <summary>
	/// This functionality is for admins only and is secured by an Authorize attribute on the controller AND by a role check in the constructor of this class.
	/// </summary>
    public class AdminService : IAdminService
    {
	    private readonly ConsultationsContext _dbContext;
	    private readonly IWebHostEnvironment _hostingEnvironment;
	    private readonly IUserService _userService;

	    public AdminService(ConsultationsContext dbContext, IWebHostEnvironment hostingEnvironment, IUserService userService)
	    {
		    _userService = userService;
			if (!_userService.IsAllowedAccess(new List<string> {"Administrator"}).Valid)
		    {
			    throw new AuthenticationException("Not authenticated");
			}
		    _dbContext = dbContext;
		    _hostingEnvironment = hostingEnvironment;
	    }

	    //public int DeleteAllData()
	    //{
		   // if (_hostingEnvironment.IsProduction())
		   // {
			  //  throw new Exception("Not allowed to do this on production");
		   // }
		   // return _dbContext.DeleteEverything();
	    //}

	    public int DeleteAllSubmissionsFromUser(string usersSubmissionsToDelete)
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

	    public int InsertQuestionsForQSConsultation(int consultationId)
	    {
		    return _dbContext.InsertQuestionsWithScriptForQSConsultation(consultationId);
	    }

		public IList<object> GetData(string tableName)
	    {
		    return _dbContext.GetAllOfATable(tableName);
	    }

		public IEnumerable<string> GetUniqueUsers()
		{
			return _dbContext.GetUniqueUsers();
		}
    }
}
