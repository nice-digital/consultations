using System;
using System.Collections.Generic;
using NICE.Identity.Authentication.Sdk.Domain;

namespace Comments.ViewModels
{
	public class User
    {
	    public User() {}

	    public User(bool isAuthenticated, string displayName, string userId, IEnumerable<Organisation> organisationsAssignedAsLead, IEnumerable<int> organisationUserIds)
		{
            IsAuthenticated = isAuthenticated;
            DisplayName = displayName;
            UserId = userId;
	        OrganisationsAssignedAsLead = organisationsAssignedAsLead;
	        OrganisationUserIds = organisationUserIds;
		}

        public bool IsAuthenticated { get; private set; }
        public string DisplayName { get; private set; }
        public string UserId { get; private set; }
		public IEnumerable<int> OrganisationUserIds { get; private set; } 

		public IEnumerable<Organisation> OrganisationsAssignedAsLead { get; private set; }
    }
}
