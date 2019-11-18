using System;
using NICE.Identity.Authentication.Sdk.Configuration;

namespace Comments.Configuration
{
    public class AuthenticationConfig
	{
        public string ApiIdentifier { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AuthorisationServiceUri { get; set; }
        public string Domain { get; set; }
        public string PostLogoutRedirectUri { get; set; }
        public string RedirectUri { get; set; }
        public string CallBackPath { get; set; }

		/// <summary>
		/// This returns the IAuthConfiguration object required for IdAM.
		/// </summary>
		/// <returns></returns>
        public IAuthConfiguration GetAuthConfiguration()
        {
			return new AuthConfiguration(
				tenantDomain: Domain,
				clientId: ClientId,
				clientSecret: ClientSecret,
				redirectUri: RedirectUri,
				postLogoutRedirectUri: PostLogoutRedirectUri,
				apiIdentifier:ApiIdentifier,
				authorisationServiceUri: AuthorisationServiceUri,
				grantType: null,
				callBackPath: CallBackPath
				);
        }
	}
}
