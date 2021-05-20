using Comments.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Comments.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Comments.Test.Infrastructure
{
    public class StubUserService : IUserService
    {
        private User _currentUser;
        public StubUserService(User currentUser)
        {
	        _currentUser = currentUser;
        }
        public User GetCurrentUser()
        {
	        return _currentUser;
        }

        public SignInDetails GetCurrentUserSignInDetails(string returnURL, IUrlHelper urlHelper)
        {
	        throw new NotImplementedException();
        }

        public Task<string> GetDisplayNameForUserId(string userId)
        {
	        throw new NotImplementedException();
        }

        public Task<string> GetEmailForUserId(string userId)
        {
	        throw new NotImplementedException();
        }

        public Task<Dictionary<string, (string displayName, string emailAddress)>> GetUserDetailsForUserIds(IEnumerable<string> userIds)
        {
	        throw new NotImplementedException();
        }

        public ICollection<string> GetUserRoles()
        {
	        throw new NotImplementedException();
        }

        public Validate IsAllowedAccess(ICollection<string> permittedRoles, User user = null)
        {
	        throw new NotImplementedException();
        }

        public (string userId, string displayName, string emailAddress) GetCurrentUserDetails()
        {
	        throw new NotImplementedException();
        }
    }
}
