using System;
using NICE.Identity.Authentication.Sdk.Configuration;

namespace Comments.Configuration
{
    public class AuthenticationConfig
	{
		public class RedisConfiguration
		{
			public string ConnectionString { get; set; }
			public bool? Enabled { get; set; }
		}

		public string ApiIdentifier { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AuthorisationServiceUri { get; set; }
        public string Domain { get; set; }
        public string PostLogoutRedirectUri { get; set; }
        public string RedirectUri { get; set; }
        public string CallBackPath { get; set; }
        public string LoginPath { get; set; }
		public string LogoutPath { get; set; }
		public string GoogleTrackingId { get; set; }

		public RedisConfiguration RedisServiceConfiguration { get; set; }

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
				callBackPath: CallBackPath,
				loginPath: LoginPath,
				logoutPath: LogoutPath,
				redisConnectionString: RedisServiceConfiguration?.ConnectionString,
				redisEnabled: RedisServiceConfiguration?.Enabled ?? false,
				googleTrackingId: GoogleTrackingId	
			);
        }
	}
}
