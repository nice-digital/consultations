using System;
using System.Collections.Generic;
using NICE.Identity.Authentication.Sdk.Domain;

namespace Comments.ViewModels
{
	public class User
    {
	    public User() {}

	    public User(bool isAuthorised, string displayName, string userId, IEnumerable<Organisation> organisationsAssignedAsLead, int? organisationUserId)
		{
            IsAuthorised = isAuthorised;
            DisplayName = displayName;
            UserId = userId;
	        OrganisationsAssignedAsLead = organisationsAssignedAsLead;
	        OrganisationUserId = organisationUserId;
		}

        public bool IsAuthorised { get; private set; }
        public string DisplayName { get; private set; }
        public string UserId { get; private set; }
		public int? OrganisationUserId { get; private set; } 

		public IEnumerable<Organisation> OrganisationsAssignedAsLead { get; private set; }
    }
}
