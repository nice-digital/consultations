using System;
using Comments.Configuration;
using Microsoft.AspNetCore.Http;
using NICE.Auth.NetCore.Models;
using NICE.Auth.NetCore.Services;

namespace Comments.Auth
{
    /// <summary>
    /// This is just a wrapper for the AuthenticateService in the nuget package. it's only needed because the authenticate service doesn't have a default constructor, which would be fine
    /// for the happy path, but it's not good for the tests as then the test startup configureservices breaks since there's a newed up authenticateservice in there.
    /// </summary>
    public class AuthService : IAuthenticateService
    {
        private readonly IAuthenticateService _authenticateService;

        public AuthService()
        {
            _authenticateService = new AuthenticateService(AppSettings.GilliamConfig);
        }

        public bool Authenticate(HttpContext httpContext, out string redirectURL, out string errorMessage)
        {
            return _authenticateService.Authenticate(httpContext, out redirectURL, out errorMessage);
        }

        public string GetLoginURL(string returnURL = null)
        {
            return _authenticateService.GetLoginURL(returnURL);
        }

        public string GetLogoutURL(string returnURL = null)
        {
            return _authenticateService.GetLogoutURL(returnURL);
        }

	    public string GetRegisterURL(string returnURL = null)
	    {
			return _authenticateService.GetRegisterURL(returnURL);
		}

	    public UserInfo FindUser(Guid userId)
	    {
		    return _authenticateService.FindUser(userId);
	    }
    }
}
