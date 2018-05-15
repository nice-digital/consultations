using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comments.ViewModels
{
    public class SecureTestViewModel
    {
        public SecureTestViewModel(string signInUrl, string signOutUrl, bool isLoggedIn, Guid? userId, string displayName, string givenName, string surname, string email, string organisation, string pageDescription)
        {
            SignInUrl = signInUrl;
            SignOutUrl = signOutUrl;
            IsLoggedIn = isLoggedIn;
            UserId = userId;
            DisplayName = displayName;
            GivenName = givenName;
            Surname = surname;
            Email = email;
            Organisation = organisation;
            PageDescription = pageDescription;
        }

        public string SignInUrl { get; private set; }
        public string SignOutUrl { get; private set; }

        public bool IsLoggedIn { get; private set; }

        public Guid? UserId { get; private set; }
        public string DisplayName { get; private set; }
        public string GivenName { get; private set; }
        public string Surname { get; private set; }
        public string Email { get; private set; }
        public string Organisation { get; private set; }
        public string PageDescription { get; private set; }
    }
}
