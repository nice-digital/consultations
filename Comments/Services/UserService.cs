using System;
using System.Collections.Generic;
using System.Linq;
using Comments.Common;
using Comments.ViewModels;
using Microsoft.AspNetCore.Http;
using NICE.Auth.NetCore.Helpers;
using NICE.Auth.NetCore.Services;

namespace Comments.Services
{
    public interface IUserService
    {
        User GetCurrentUser();
	    SignInDetails GetCurrentUserSignInDetails(string returnURL);
	    string GetDisplayNameForUserId(Guid userId);
	    IDictionary<Guid, string> GetDisplayNamesForMultipleUserIds(IEnumerable<Guid> userIds);
    }

    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
	    private readonly IAuthenticateService _authenticateService;

	    public UserService(IHttpContextAccessor httpContextAccessor, IAuthenticateService authenticateService)
	    {
		    _httpContextAccessor = httpContextAccessor;
		    _authenticateService = authenticateService;
	    }

        public User GetCurrentUser()
        {
            var contextUser = _httpContextAccessor.HttpContext?.User;
            return new User(contextUser?.Identity.IsAuthenticated ?? false, contextUser?.DisplayName(), contextUser?.Id());
        }

	    public SignInDetails GetCurrentUserSignInDetails(string returnURL)
	    {
			var user = GetCurrentUser();

		    var signInURL = _authenticateService.GetLoginURL(returnURL.ToConsultationsRelativeUrl());
		    var registerURL = _authenticateService.GetRegisterURL(returnURL.ToConsultationsRelativeUrl());

			return new SignInDetails(user, signInURL, registerURL);
		}

	    public string GetDisplayNameForUserId(Guid userId)
	    {
		    return _authenticateService.FindUser(userId)?.DisplayName;
	    }

	    public IDictionary<Guid, string> GetDisplayNamesForMultipleUserIds(IEnumerable<Guid> userIds)
	    {
		    return userIds.Distinct().ToDictionary(userId => userId, GetDisplayNameForUserId);
	    }
    }
}
