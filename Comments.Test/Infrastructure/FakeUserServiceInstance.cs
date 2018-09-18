using System;
using System.Collections.Generic;
using System.Text;
using Comments.Services;
using Comments.ViewModels;

namespace Comments.Test.Infrastructure
{
	class FakeUserServiceInstance : IUserService
	{
		public User GetCurrentUser()
		{
			return new User(true, "Benjamin Button", Guid.Empty, "Org");
		}

		public SignInDetails GetCurrentUserSignInDetails(string returnURL)
		{
			throw new NotImplementedException();
		}

		public string GetDisplayNameForUserId(Guid userId)
		{
			throw new NotImplementedException();
		}

		public IDictionary<Guid, string> GetDisplayNamesForMultipleUserIds(IEnumerable<Guid> userIds)
		{
			throw new NotImplementedException();
		}
	}
}
