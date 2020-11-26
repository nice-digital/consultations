using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authentication.Sdk.Configuration;
using NICE.Identity.Authentication.Sdk.Domain;
using System.Threading.Tasks;

namespace Comments.Test.Infrastructure
{
	public class FakeAPITokenService : IApiToken
	{
		private readonly JwtToken _tokenToReturn;

		public FakeAPITokenService(JwtToken tokenToReturn = null)
		{
			_tokenToReturn = tokenToReturn ?? new JwtToken() { AccessToken = "FAKE ACCESS TOKEN" };
		}

		public Task<JwtToken> GetAccessToken(string domain, string clientId, string clientSecret, string audience)
		{
			return Task.Run(() => _tokenToReturn);
		}

		public Task<JwtToken> GetAccessToken(IAuthConfiguration authConfiguration)
		{
			return Task.Run(() => _tokenToReturn);
		}
	}
}
