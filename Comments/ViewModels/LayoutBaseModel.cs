using Comments.Configuration;
using System.Security.Principal;

namespace Comments.ViewModels
{
	public class LayoutBaseModel
	{
		public LayoutBaseModel(IPrincipal user, string signInURL, string signOutURL)
		{
			IsAuthenticated = user != null && user.Identity != null && user.Identity.IsAuthenticated;
			DisplayName = user?.Identity.Name ?? "";

			AccountsEnvironment = AppSettings.Environment.AccountsEnvironment.ToLower();

			GlobalNavScript = AppSettings.GlobalNavConfig.Script;
			GlobalNavScriptIE8 = AppSettings.GlobalNavConfig.ScriptIE8;

			LinkText = IsAuthenticated ? "Sign out" : "Sign in";
			LinkURL = IsAuthenticated ? signOutURL : signInURL;

			CookieBannerScript = AppSettings.GlobalNavConfig.CookieBannerScript;
		}

		public bool IsAuthenticated { get; private set; }

		public string AccountsEnvironment { get; private set; }

		public string GlobalNavScript { get; private set; }

		public string GlobalNavScriptIE8 { get; private set; }

		public string LinkText { get; private set; }

		public string LinkURL { get; private set; }

		public string DisplayName { get; private set; }

		public string CookieBannerScript { get; private set; }
	}
}
