using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Comments.Configuration;
using Comments.Models;
using Comments.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Comments.Common
{
    public static class Extensions
    {
        /// <summary>
        /// This isn't a consultations uri. this is the relative url, shown in the address bar.
        /// This function converts a relative url like "/1/1/introduction" into "/consultations/1/1/introduction".
        /// If the latter is passed in, then it returns it straight.
        /// </summary>
        /// <param name="relativeURL"></param>
        /// <returns></returns>
        public static string ToConsultationsRelativeUrl(this string relativeURL)
        {
            relativeURL = relativeURL.ToLower();

            if (relativeURL.StartsWith(Constants.ConsultationsBasePath))
                return relativeURL;

            var combinedPath = UrlCombine(Constants.ConsultationsBasePath, relativeURL);

            return combinedPath;
        }

        private static string UrlCombine(string url1, string url2)
        {
            if (url1.Length == 0)
            {
                return url2;
            }
            if (url2.Length == 0)
            {
                return url1;
            }
            url1 = url1.TrimEnd('/', '\\');
            url2 = url2.TrimStart('/', '\\');

            return string.Format("{0}/{1}", url1, url2);
        }

        /// <summary>
        /// Swaps out http:// for https:// in a url string.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ToHTTPS(this string url)
        {
            return (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                ? url.Replace("http://", "https://", StringComparison.OrdinalIgnoreCase)
                : url;
        }

		public static void Clear<T>(this DbSet<T> dbSet) where T : class
		{
			dbSet.RemoveRange(dbSet);
		}

	    private const string UnknownHostName = "UNKNOWN-HOST";
		public static Uri GetUri(this HttpRequest request)
	    {
		    if (null == request)
		    {
			    throw new ArgumentNullException("request");
		    }

		    if (true == string.IsNullOrWhiteSpace(request.Scheme))
		    {
			    throw new ArgumentException("Http request Scheme is not specified");
		    }

		    string hostName = request.Host.HasValue ? request.Host.ToString() : UnknownHostName;

		    var builder = new StringBuilder();

		    builder.Append(request.Scheme)
			    .Append("://")
			    .Append(hostName);

		    if (true == request.Path.HasValue)
		    {
			    builder.Append(request.Path.Value);
		    }

		    if (true == request.QueryString.HasValue)
		    {
			    builder.Append(request.QueryString);
		    }

		    return new Uri(builder.ToString());
	    }

	    public static bool IsIntegrationTest(this IHostingEnvironment hostingEnvironment)
	    {
		    return hostingEnvironment.ContentRootPath.IndexOf(".Test", StringComparison.OrdinalIgnoreCase) != -1;
	    }

	    public static IEnumerable<string> FilterRoles(this IEnumerable<string> roles)
	    {
		    if (roles == null)
			    return new List<string>();

		    var roleList = roles.ToList();
		    if (!roleList.Any())
			    return new List<string>();

		    return roleList.Where(role => AppSettings.ConsultationListConfig.DownloadRoles.AllRoles.Contains(role));
	    }
		
	    public static long ToJavaScriptTicksSinceEpoch(this DateTime datetime)
	    {
			var javascriptEpochTicks = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks; //c#'s date epoch is MinDate, whereas js is 1-1-1970
		    return (datetime.ToUniversalTime().Ticks - javascriptEpochTicks) / 10000;
	    }

	    public static IDictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> dictA, IDictionary<TKey, TValue> dictB) where TValue : class
	    {
		    return dictA.Keys.Union(dictB.Keys).ToDictionary(k => k, k => dictA.ContainsKey(k) ? dictA[k] : dictB[k]);
	    }

	    public static Dictionary<int, Guid> GetSessionCookies(this IRequestCookieCollection requestCookies)
	    {
		    var sessions = new Dictionary<int, Guid>();
			if (requestCookies != null && requestCookies.Count > 0)
		    {
			    var cookies = requestCookies.Where(cookie => cookie.Key.StartsWith(Constants.SessionCookieName)).ToList();

			    if (cookies.Any())
			    {
				    foreach (var (cookieKey, cookieValue) in cookies)
				    {
					    if (int.TryParse(cookieKey.Substring(Constants.SessionCookieName.Length), out var consultationId)
					        && Guid.TryParse(cookieValue, out var sessionId)
					        && !sessions.ContainsKey(consultationId))
					    {
						    sessions.Add(consultationId, sessionId);
					    }
				    }
			    }
		    }
			return sessions;
	    }

	    public static IList<ValidatedSession> ValidatedSessions(this ClaimsPrincipal claimsPrincipal)
	    {
		    var serialisedSessions = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type.Equals(Constants.OrgansationAuthentication.ValidatedSessionsClaim) && claim.Issuer.Equals(Constants.OrgansationAuthentication.Issuer))?.Value;
		    if (string.IsNullOrEmpty(serialisedSessions))
		    {
			    return new List<ValidatedSession>();
		    }

		    return JsonConvert.DeserializeObject<IList<ValidatedSession>>(serialisedSessions);
	    }
	}
}
