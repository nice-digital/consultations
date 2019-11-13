using Comments.Common;
using Comments.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using NICE.Identity.Authentication.Sdk.API;
using NICE.Identity.Authentication.Sdk.Extensions;

namespace Comments.Services
{
	public interface IUserService
    {
        User GetCurrentUser();
		SignInDetails GetCurrentUserSignInDetails(string returnURL);
		Task<string> GetDisplayNameForUserId(string userId);
		Task<string> GetEmailForUserId(string userId);
		Task<Dictionary<string, (string displayName, string emailAddress)>> GetUserDetailsForUserIds(IEnumerable<string> userIds);
	    ICollection<string> GetUserRoles();
	    Validate IsAllowedAccess(ICollection<string> permittedRoles);
	}

    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;
        private readonly IAPIService _apiService;

        public UserService(IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator, IAPIService apiService)
        {
	        _httpContextAccessor = httpContextAccessor;
	        _linkGenerator = linkGenerator;
	        _apiService = apiService;
        }

        public User GetCurrentUser()
        {
            var contextUser = _httpContextAccessor.HttpContext?.User;

            return new User(contextUser?.Identity.IsAuthenticated ?? false, contextUser?.DisplayName(), contextUser?.NameIdentifier());
        }

		public SignInDetails GetCurrentUserSignInDetails(string returnURL)
	    {
			var user = GetCurrentUser();

		    var signInURL = _linkGenerator.GetPathByAction(_httpContextAccessor.HttpContext, Constants.Auth.LoginAction, Constants.Auth.ControllerName, new { returnURL = returnURL.ToConsultationsRelativeUrl() });
		    var registerURL = "TODO: register url";
			
			return new SignInDetails(user, signInURL, registerURL);
		}

	    public async Task<Dictionary<string, (string displayName, string emailAddress)>> GetUserDetailsForUserIds(IEnumerable<string> userIds)
	    {
		    var users = await _apiService.FindUsers(userIds);
		    return users.ToDictionary(user => user.NameIdentifier, user => (displayName: user.DisplayName,  emailAddress : user.EmailAddress));
	    }

	    public async Task<string> GetDisplayNameForUserId(string userId)
		{
			var user = await GetUserDetailsForUserIds(new List<string> {userId});
			return user[userId].displayName;
		}

		public async Task<string> GetEmailForUserId(string userId)
	    {
		    var users = await _apiService.FindUsers(new List<string> { userId });
		    return users?.FirstOrDefault()?.EmailAddress;
	    }

	    public ICollection<string> GetUserRoles()
	    {
			var niceUser = _httpContextAccessor.HttpContext?.User;
			var host = _httpContextAccessor.HttpContext?.Request.Host.Host;
			return niceUser?.Roles(host).ToList() ?? new List<string>();
	    }

	    public Validate IsAllowedAccess(ICollection<string> permittedRoles)
	    {
		    if (!permittedRoles.Any())
		    {
			    throw new ArgumentException("There is expected to be at least one permitted role", nameof(permittedRoles));
		    }

		    var user = GetCurrentUser();
		    if (!user.IsAuthorised)
		    {
			    return new Validate(false, true, false, "User is not authorised");
		    }
		    var niceUser = _httpContextAccessor.HttpContext.User;
		    if (!niceUser.Identity.IsAuthenticated)
		    {
			    return new Validate(false, false, false, "Not authenticated");
		    }
		    var host = _httpContextAccessor.HttpContext?.Request.Host.Host;
			var userRoles = niceUser.Roles(host);

		    if (!userRoles.Any(permittedRoles.Contains))
		    {
			    return new Validate(false, true, false, "NICE user is not permitted to download this file");
		    }
		    return new Validate(valid: true);
	    }
	}
}
