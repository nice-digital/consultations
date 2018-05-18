using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comments.ViewModels
{
    public class User
    {
        public User(bool isAuthorised, string displayName, Guid? userId)
        {
            IsAuthorised = isAuthorised;
            DisplayName = displayName;
            UserId = userId;
        }

        public bool IsAuthorised { get; private set; }
        public string DisplayName { get; private set; }
        public Guid? UserId { get; private set; }
    }
}
