using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comments.ViewModels
{
    public class User
    {
	    public User() {}

	    public User(bool isAuthorised, string displayName, string userId, string organisationName = null)
        {
            IsAuthorised = isAuthorised;
            DisplayName = displayName;
            UserId = userId;
	        OrganisationName = organisationName;
        }

        public bool IsAuthorised { get; private set; }
        public string DisplayName { get; private set; }
        public string UserId { get; private set; }
		public string OrganisationName { get; private set; }
    }
}
