using NICE.Identity.Authentication.Sdk.Domain;
using NICE.Identity.Authentication.Sdk.TokenStore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comments.Test.Infrastructure
{
	public class FakeApiTokenStore : IApiTokenStore
	{
		Dictionary<Guid, JwtToken> _tokens = new Dictionary<Guid, JwtToken>();

		public Task<string> StoreAsync(JwtToken token)
		{
			var newId = Guid.NewGuid();
			_tokens.Add(newId, token);
			return Task.Run(() => newId.ToString());
		}

		public Task RenewAsync(string key, JwtToken token)
		{
			throw new NotImplementedException();
		}

		public Task<JwtToken> RetrieveAsync(string key)
		{
			return Task.Run(() => _tokens[Guid.Parse(key)]);
		}

		public Task RemoveAsync(string key)
		{
			throw new NotImplementedException();
		}
	}
}
