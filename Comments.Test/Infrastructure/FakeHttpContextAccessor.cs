using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Features;
using NICE.Identity.Authentication.Sdk.Domain;
using Claim = System.Security.Claims.Claim;

namespace Comments.Test.Infrastructure
{
    public static class FakeHttpContextAccessor
    {
        public static IHttpContextAccessor Get(bool isAuthenticated, string displayName = null, string userId = null, TestUserType testUserType = TestUserType.NotAuthenticated)
        {
	        var context = new Mock<HttpContext>();

            if (isAuthenticated || testUserType == TestUserType.Authenticated || testUserType == TestUserType.Administrator || testUserType == TestUserType.IndevUser)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimType.DisplayName, displayName, null, AuthenticationConstants.IdAMIssuer),
                    new Claim(ClaimType.NameIdentifier, userId.ToString(), null, AuthenticationConstants.IdAMIssuer),
	                new Claim(ClaimType.Role, "IndevUser", null, AuthenticationConstants.IdAMIssuer),
	               // new Claim(ClaimType.Role, "Administrator", null, "http://consultations.nice.org.uk")
				};
				switch (testUserType)
				{
					case TestUserType.IndevUser:
						claims.Add(new Claim(ClaimType.Role, "IndevUser", null, "http://consultations.nice.org.uk"));
						break;
					case TestUserType.Administrator:
						claims.Add(new Claim(ClaimType.Role, "Administrator", null, "http://consultations.nice.org.uk"));
						break;
					case TestUserType.CustomFictionalRole:
						claims.Add(new Claim(ClaimType.Role, "CustomFictionalRole", null, "http://consultations.nice.org.uk"));
						break;
					case TestUserType.ConsultationListTestRole:
						claims.Add(new Claim(ClaimType.Role, "ConsultationListTestRole", null, "http://consultations.nice.org.uk"));
						break;
				}

				context.Setup(r => r.User)
                    .Returns(() => new ClaimsPrincipal(new ClaimsIdentity(claims, AuthenticationConstants.AuthenticationScheme)));

				context.Setup(r => r.Features)
					.Returns(() => new FeatureCollection());
            }
            else
            {
                context.Setup(r => r.User).Returns(() => null);
            }

            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(ca => ca.HttpContext).Returns(context.Object);

            return contextAccessor.Object;
        }

		
	}
}
