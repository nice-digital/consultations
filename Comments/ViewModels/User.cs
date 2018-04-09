using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comments.ViewModels
{
    public class User
    {
        public User(bool isLoggedIn, string displayName, Guid? userId)
        {
            IsLoggedIn = isLoggedIn;
            DisplayName = displayName;
            UserId = userId;
        }

        public bool IsLoggedIn { get; private set; }
        public string DisplayName { get; private set; }
        public Guid? UserId { get; private set; }
    }
}
