using System;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NICE.Auth.NetCore.Services;
using NICE.Auth.NetCore.Helpers;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Comments.Auth
{
    public class AuthHandler : AuthenticationHandler<AuthOptions>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthenticateService _authenticateService;

        public AuthHandler(IOptionsMonitor<AuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IHttpContextAccessor httpContextAccessor, IAuthenticateService authenticateService)
            : base(options, logger, encoder, clock)
        {
            _httpContextAccessor = httpContextAccessor;
            _authenticateService = authenticateService;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var requestPath = httpContext.Request.Path.HasValue ? httpContext.Request.Path.Value : null;
                if (requestPath == null || requestPath.IndexOf("/api/", StringComparison.OrdinalIgnoreCase) == -1)
                    return Task.FromResult(AuthenticateResult.NoResult());

                var authenticated = httpContext.User?.Identity != null && httpContext.User.Identity.IsAuthenticated;

                if (!authenticated && !httpContext.Items.ContainsKey(Constants.ItemsAuthAttempted))
                {
                    authenticated = _authenticateService.Authenticate(httpContext, out var redirectURL);
                    httpContext.Items[Constants.ItemsAuthAttempted] = true;
                    if (authenticated && !string.IsNullOrWhiteSpace(redirectURL))
                    {
                        //httpContext.Response.Redirect(redirectURL);                        
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
                var redirectUrl = _authenticateService.GetLoginURL(returnURL.AbsoluteUri);
                httpContext.Response.Redirect(redirectUrl);
            }
            return Task.CompletedTask;
        }
    }
}
