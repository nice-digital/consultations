using Comments.Services;
using System;

namespace Comments.Test.Infrastructure
{
    public static class FakeUserService
    {
        public static IUserService Get(bool isAuthenticated, string displayName = null, string userId = null, TestUserType testUserType  = TestUserType.NotAuthenticated, bool addRoleClaim = true)
        {
            return new UserService(FakeHttpContextAccessor.Get(isAuthenticated, displayName, userId, testUserType, addRoleClaim), new FakeAPIService());
        }
    }
}
