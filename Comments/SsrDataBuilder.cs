using System.Collections.Generic;
using System.Linq;
using Comments.Common;
using Comments.Configuration;
using Comments.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authentication.Sdk.Extensions;

namespace Comments
{
    public static class SsrDataBuilder
    {
        public static object Build(HttpContext httpContext, string htmlTemplate, LinkGenerator linkGenerator)
        {
            var user = new User(httpContext.User);

            var cookiesForSSR = httpContext.Request.Cookies
                .Where(cookie =>
                    cookie.Key.StartsWith(AuthenticationConstants.CookieName) ||
                    cookie.Key.StartsWith(Constants.SessionCookieName))
                .ToList();

            var cookies = cookiesForSSR.Any()
                ? $"{string.Join("; ", cookiesForSSR.Select(c => $"{c.Key}={c.Value}"))};"
                : null;

            var host = httpContext.Request.Host.Host;
            var userRoles = httpContext.User?.Roles(host).ToList() ?? new List<string>();

            var isAdminUser = userRoles.Any(role =>
                AppSettings.ConsultationListConfig.DownloadRoles.AdminRoles.Contains(role));

            var teamRoles = userRoles
                .Where(role =>
                    AppSettings.ConsultationListConfig.DownloadRoles.TeamRoles.Contains(role))
                .ToList();

            var isTeamUser = !isAdminUser && teamRoles.Any();

            return new
            {
                originalHtml = htmlTemplate,
                cookies,
                isAuthorised = user.IsAuthenticatedByAccounts,
                displayName = user.DisplayName,
                isLead = user.OrganisationsAssignedAsLead?.Any(),
                isAdminUser,
                isTeamUser,
                signInURL = linkGenerator.GetPathByAction(
                    httpContext,
                    Constants.Auth.LoginAction,
                    Constants.Auth.ControllerName,
                    new { returnUrl = httpContext.Request.Path }),

                signOutURL = linkGenerator.GetPathByAction(
                    httpContext,
                    Constants.Auth.LogoutAction,
                    Constants.Auth.ControllerName),

                registerURL = linkGenerator.GetPathByAction(
                    httpContext,
                    Constants.Auth.LoginAction,
                    Constants.Auth.ControllerName,
                    new { returnUrl = httpContext.Request.Path, goToRegisterPage = true }),

                requestURL = httpContext.Request.Path.ToString(),
                accountsEnvironment = AppSettings.Environment.AccountsEnvironment
            };
        }
    }
}
