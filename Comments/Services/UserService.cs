using System;
using Comments.ViewModels;
using Microsoft.AspNetCore.Http;
using NICE.Auth.NetCore.Helpers;

namespace Comments.Services
{
    public interface IUserService
    {
        User GetCurrentUser();
    }

    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public User GetCurrentUser()
        {
            var contextUser = _httpContextAccessor.HttpContext?.User;
            return new User(contextUser?.Identity.IsAuthenticated ?? false, contextUser?.DisplayName(), contextUser?.Id());
        }
    }
}
