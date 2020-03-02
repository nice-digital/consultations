using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NICE.Auth.NetCore.Services;
using NICE.Auth.NetCore.Helpers;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Comments.Common;

namespace Comments.Auth
{
    public class AuthHandler : AuthenticationHandler<AuthOptions>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthenticateService _authenticateService;
        private readonly ILogger _logger;
        /// <summary>
        /// This is a list of paths which should authenticate. That just means that it will check for the nice accounts cookie. it doesn't mean they require authorisation, i.e. they're not protected by auth.
        /// if no cookie is found, that's fine. no redirects or anything, just no user details in the data.
        /// 
        /// I suspect this will need to change..
        /// </summary>
        private readonly List<Regex> _pathsToAuthenticate = new List<Regex>
        {
            new Regex(@"^\/?consultations\/api\/.+$", RegexOptions.IgnoreCase), //api path
			new Regex(@"\/?index.html", RegexOptions.IgnoreCase), //SSR path on the server only.
			new Regex(@"^\/?$") //root - only used on dev machines
        };

        public AuthHandler(IOptionsMonitor<AuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IHttpContextAccessor httpContextAccessor, IAuthenticateService authenticateService)
            : base(options, logger, encoder, clock)
        {
            _httpContextAccessor = httpContextAccessor;
            _authenticateService = authenticateService;
            _logger = logger.CreateLogger(this.GetType());
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
				//the problem with the below is that it doesn't work for the server-side render.
                //var requestPath = httpContext.Request.Path.HasValue ? httpContext.Request.Path.Value : null;
                //if (!ShouldAuthenticatePath(requestPath))
                //    return Task.FromResult(AuthenticateResult.NoResult());

                var authenticated = httpContext.User?.Identity != null && httpContext.User.Identity.IsAuthenticated;

                if (!authenticated && !httpContext.Items.ContainsKey(NICE.Auth.NetCore.Helpers.Constants.ItemsAuthAttempted))
                {
                    authenticated = _authenticateService.Authenticate(httpContext, out var redirectURL, out var errorMessage);
	                if (!string.IsNullOrWhiteSpace(errorMessage))
	                {
		                _logger.LogError(errorMessage);
						throw new Exception(errorMessage); //debatable. should this throw?
	                }
                    httpContext.Items[NICE.Auth.NetCore.Helpers.Constants.ItemsAuthAttempted] = true;
                    if (authenticated && !string.IsNullOrWhiteSpace(redirectURL))
                    {
                        httpContext.Response.Redirect(redirectURL);                        
                    }
                }
                    
                if (authenticated)
                {
                    var principal = httpContext.User;
                    var ticket = new AuthenticationTicket(principal, Options.Scheme);
                    return Task.FromResult(AuthenticateResult.Success(ticket));
                }
            }
            return Task.FromResult(AuthenticateResult.Fail("Not logged in"));
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var returnURL = httpContext.Request.GetUri();
                var redirectUrl = _authenticateService.GetLoginURL(returnURL.AbsoluteUri.ToHTTPS());
                httpContext.Response.Redirect(redirectUrl);
            }
            return Task.CompletedTask;
        }

        private bool ShouldAuthenticatePath(string requestPath)
        {
            if (requestPath == null)
                return true;

            foreach (var regex in _pathsToAuthenticate)
            {
                if (regex.IsMatch(requestPath))
                {
                    return true;
                }
            }
            return false;
        }
    }
}