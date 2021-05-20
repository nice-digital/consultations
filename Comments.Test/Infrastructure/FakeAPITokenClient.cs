using NICE.Identity.Authentication.Sdk.Authorisation;
using NICE.Identity.Authentication.Sdk.Configuration;
using System.Threading.Tasks;

namespace Comments.Test.Infrastructure
{
	public class FakeAPITokenClient : IApiTokenClient
	{
		private readonly string _tokenToReturn;

		public FakeAPITokenClient(string accessToken = null)
		{
			_tokenToReturn = accessToken ?? "FAKE ACCESS TOKEN";
		}

		public Task<string> GetAccessToken()
		{
			return Task.Run(() => _tokenToReturn);
		}

		public Task<string> GetAccessToken(IAuthConfiguration authConfiguration)
		{
			return GetAccessToken();
		}

		public Task<string> GetAccessToken(string domain, string clientId, string clientSecret, string audience)
		{
			return GetAccessToken();
		}
	}
}
