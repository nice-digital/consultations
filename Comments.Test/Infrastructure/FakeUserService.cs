using Comments.Services;
using System;

namespace Comments.Test.Infrastructure
{
    public static class FakeUserService
    {
        public static IUserService Get(bool isAuthenticated, string displayName = null, Guid? userId = null, TestUserType testUserType  = TestUserType.NotAuthenticated)
        {
            return new UserService(FakeHttpContextAccessor.Get(isAuthenticated, displayName, userId, testUserType), new FakeAuthenticateService(authenticated: isAuthenticated));
        }
    }
}
