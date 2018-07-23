using Microsoft.AspNetCore.Http;
using Moq;
using NICE.Auth.NetCore.Helpers;
using NICE.Auth.NetCore.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Claim = System.Security.Claims.Claim;

namespace Comments.Test.Infrastructure
{
    public static class FakeHttpContextAccessor
    {
        public static IHttpContextAccessor Get(bool isAuthenticated, string displayName = null, Guid? userId = null)
        {
            var context = new Mock<HttpContext>();

            if (isAuthenticated)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimType.Name, displayName, null, "http://consultations.nice.org.uk"),
                    new Claim(ClaimType.NameIdentifier, userId.ToString(), null, "http://consultations.nice.org.uk")
                };
                context.Setup(r => r.User)
                    .Returns(() => new ClaimsPrincipal(new ClaimsIdentity(claims, Constants.DefaultScheme)));
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
