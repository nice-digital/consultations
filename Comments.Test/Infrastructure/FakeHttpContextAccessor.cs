using Comments.Common;
using Comments.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Moq;
using Newtonsoft.Json;
using NICE.Identity.Authentication.Sdk.Domain;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Claim = System.Security.Claims.Claim;

namespace Comments.Test.Infrastructure
{
	public static class FakeHttpContextAccessor
    {
	    private static string AuthenticationTokenExtensions_TokenKeyPrefix = ".Token.";
		public static IHttpContextAccessor Get(bool isAuthenticated, string displayName = null, string userId = null, TestUserType testUserType = TestUserType.NotAuthenticated, bool addRoleClaim = true, int? organisationUserId = null, int? organisationIdUserIsLeadOf = null, string emailAddress = null)
        {
	        var context = new Mock<HttpContext>();
	        var roleIssuer = "www.example.com"; //the issuer of the role is the domain for which the role is setup.
			
			if (isAuthenticated || testUserType == TestUserType.Authenticated || testUserType == TestUserType.Administrator || testUserType == TestUserType.IndevUser)
			{
				var authenticationScheme = (string.IsNullOrEmpty(userId) && organisationUserId.HasValue) ? OrganisationCookieAuthenticationOptions.DefaultScheme : AuthenticationConstants.AuthenticationScheme;

				var claims = new List<Claim>
                {
                    new Claim(ClaimType.DisplayName, displayName, null, AuthenticationConstants.IdAMIssuer),
                };
				if (userId != null)
				{
					claims.Add(new Claim(ClaimType.NameIdentifier, userId.ToString(), null, AuthenticationConstants.IdAMIssuer));
				}
				if (addRoleClaim)
				{
					claims.Add(new Claim(ClaimType.Role, "IndevUser", null, roleIssuer));
				}

				if (organisationUserId.HasValue)
				{
					var validatedSessions = new List<ValidatedSession>() { new ValidatedSession(organisationUserId.Value, 1, Guid.NewGuid(), 1)};
					claims.Add(new Claim(Constants.OrgansationAuthentication.ValidatedSessionsClaim, JsonConvert.SerializeObject(validatedSessions), null, Constants.OrgansationAuthentication.Issuer));
				}

				if (organisationIdUserIsLeadOf.HasValue)
				{
					var organisations = new List<Organisation>() { new Organisation(organisationIdUserIsLeadOf.Value, "Test org", isLead: true)};
					claims.Add(new Claim(ClaimType.Organisations, JsonConvert.SerializeObject(organisations), "www.nice.org.uk", AuthenticationConstants.IdAMIssuer));
				}

				if (emailAddress != null)
				{
					claims.Add(new Claim(ClaimType.EmailAddress, emailAddress, null, AuthenticationConstants.IdAMIssuer));
				}

				switch (testUserType)
				{
					case TestUserType.IndevUser:
						claims.Add(new Claim(ClaimType.Role, "IndevUser", null, roleIssuer));
						break;
					case TestUserType.Administrator:
						claims.Add(new Claim(ClaimType.Role, "Administrator", null, roleIssuer));
						break;
					case TestUserType.CustomFictionalRole:
						claims.Add(new Claim(ClaimType.Role, "CustomFictionalRole", null, roleIssuer));
						break;
					case TestUserType.ConsultationListTestRole:
						claims.Add(new Claim(ClaimType.Role, "ConsultationListTestRole", null, roleIssuer));
						break;
				}

				var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationScheme));
				context.Setup(r => r.User)
                    .Returns(() => new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationScheme)));

				var authServiceMock = new Mock<IAuthenticationService>();
				authServiceMock
					.Setup(_ => _.AuthenticateAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
					.Returns(Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal,
						new AuthenticationProperties(new Dictionary<string, string>() { { AuthenticationTokenExtensions_TokenKeyPrefix + "access_token", "fake access token" } }), authenticationScheme))));


				var serviceProviderMock = new Mock<IServiceProvider>();

				serviceProviderMock
					.Setup(_ => _.GetService(typeof(IAuthenticationService)))
					.Returns(authServiceMock.Object);

				context.Setup(r => r.RequestServices).Returns(serviceProviderMock.Object);
			} else if (organisationUserId.HasValue)
			{
				var authenticationScheme = OrganisationCookieAuthenticationOptions.DefaultScheme;

				var validatedSessions = new List<ValidatedSession>() { new ValidatedSession(organisationUserId.Value, 1, Guid.NewGuid(), 1) };
				var claims = new List<Claim> {new Claim(Constants.OrgansationAuthentication.ValidatedSessionsClaim, JsonConvert.SerializeObject(validatedSessions), null, Constants.OrgansationAuthentication.Issuer)};

				var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationScheme));
				context.Setup(r => r.User)
					.Returns(() => new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationScheme)));

				var authServiceMock = new Mock<IAuthenticationService>();
				authServiceMock
					.Setup(_ => _.AuthenticateAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
					.Returns(Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal,
						new AuthenticationProperties(new Dictionary<string, string>() { { AuthenticationTokenExtensions_TokenKeyPrefix + "access_token", "fake access token" } }), authenticationScheme))));


				var serviceProviderMock = new Mock<IServiceProvider>();

				serviceProviderMock
					.Setup(_ => _.GetService(typeof(IAuthenticationService)))
					.Returns(authServiceMock.Object);

				context.Setup(r => r.RequestServices).Returns(serviceProviderMock.Object);
			}
			else
            {
                context.Setup(r => r.User).Returns(() => null);
            }

            context.Setup(r => r.Features)
	            .Returns(() => new FeatureCollection());

            var defaultHttpRequest = new DefaultHttpContext().Request;
            defaultHttpRequest.Host = new HostString(roleIssuer);

            context.Setup(r => r.Request)
	            .Returns(defaultHttpRequest);


			var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(ca => ca.HttpContext).Returns(context.Object);

            return contextAccessor.Object;
        }


	}
}
