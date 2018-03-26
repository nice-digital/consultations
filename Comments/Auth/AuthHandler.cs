using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Comments.Auth
{
    public class AuthHandler : AuthenticationHandler<AuthOptions>
    {
        public AuthHandler(IOptionsMonitor<AuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            //return Task.FromResult(AuthenticateResult.Fail("this returns a 401"));

            //var identities = new List<ClaimsIdentity> { new ClaimsIdentity(AuthOptions.DefaultScheme) };
            //var ticket = new AuthenticationTicket(new ClaimsPrincipal(identities), Options.Scheme);

            var ticket = new AuthenticationTicket(new ClaimsPrincipal(), Options.Scheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
