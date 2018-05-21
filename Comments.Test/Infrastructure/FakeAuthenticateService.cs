using Microsoft.AspNetCore.Http;
using NICE.Auth.NetCore.Services;

namespace Comments.Test.Infrastructure
{
    public class FakeAuthenticateService : IAuthenticateService
    {
        private readonly bool _authenticated;
        private readonly string _redirectURL;
        private readonly string _loginURL;
        private readonly string _logoutURL;
	    private readonly string _registerURL;

		public FakeAuthenticateService(bool authenticated = true, string redirectURL = "consultations://./consultation/1/document/1/chapter/introduction", 
            string loginURL = "/signin?returnURL=/", string logoutURL = "/signout?returnURL=/", string registerURL = "/register?returnURL=/")
        {
            _authenticated = authenticated;
            _redirectURL = redirectURL;
            _loginURL = loginURL;
            _logoutURL = logoutURL;
	        _registerURL = registerURL;
        }

        public bool Authenticate(HttpContext httpContext, out string redirectURL)
        {
            redirectURL = _redirectURL;
            return _authenticated;
        }

        public string GetLoginURL(string returnURL = null)
        {
            return _loginURL;
        }

        public string GetLogoutURL(string returnURL = null)
        {
            return _logoutURL;
        }

	    public string GetRegisterURL(string returnURL = null)
	    {
		    return _registerURL;
	    }
    }
}
