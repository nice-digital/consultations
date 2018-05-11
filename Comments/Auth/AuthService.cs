﻿using Comments.Configuration;
using Microsoft.AspNetCore.Http;
using NICE.Auth.NetCore.Services;

namespace Comments.Auth
{
    /// <summary>
    /// This is just a wrapper for the AuthenticateService in the nuget package. it's only needed because the authenticate service doesn't have a default constructor, which would be fine
    /// except for the main code, but it's not good for the tests as then the test startup configureservices breaks since there's a newed up authenticateservice in there.
    /// </summary>
    public class AuthService : IAuthenticateService
    {
        private readonly IAuthenticateService _authenticateService;

        public AuthService()
        {
            _authenticateService = new AuthenticateService(AppSettings.GilliamConfig);
        }

        public bool Authenticate(HttpContext httpContext, out string redirectURL)
        {
            return _authenticateService.Authenticate(httpContext, out redirectURL);
        }

        public string GetLoginURL(string returnURL = null, string xReferer = null)
        {
            return _authenticateService.GetLoginURL(returnURL, xReferer);
        }

        public string GetLogoutURL(string returnURL = null)
        {
            return _authenticateService.GetLogoutURL(returnURL);
        }
    }
}
