using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

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
	}
}
