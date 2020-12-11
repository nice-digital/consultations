using NICE.Identity.Authentication.Sdk.Domain;
using System.Collections.Generic;
using System.Linq;

namespace Comments.ViewModels
{
	public class User
    {
	    public User() {}

	    public User(bool isAuthenticated, string displayName, string userId, IEnumerable<Organisation> organisationsAssignedAsLead, IEnumerable<int> validatedOrganisationUserIds, IEnumerable<int> validatedConsultationIds)
		{
            IsAuthenticated = isAuthenticated;
            DisplayName = displayName;
            UserId = userId;
	        OrganisationsAssignedAsLead = organisationsAssignedAsLead;
	        ValidatedOrganisationUserIds = validatedOrganisationUserIds;
	        ValidatedConsultationIds = validatedConsultationIds;
		}

        public bool IsAuthenticated { get; private set; }
        public string DisplayName { get; private set; }
        public string UserId { get; private set; }
		
		public IEnumerable<Organisation> OrganisationsAssignedAsLead { get; private set; }

		public readonly IEnumerable<int> ValidatedOrganisationUserIds; 
		public readonly IEnumerable<int> ValidatedConsultationIds;

		private bool IsAuthenticatedByAccounts => (IsAuthenticated && UserId != null);
		private bool IsAuthenticatedByOrganisationCookie => (IsAuthenticated && UserId == null);

		/// <summary>
		/// Determines whether a user is authorised to e.g. comment, on a given consultation.
		/// Organisation cookie users are only permitted to comment on the consultation they have a validated cookie for.
		/// </summary>
		/// <param name="consultationId"></param>
		/// <returns></returns>
		public bool IsAuthorisedByConsultationId(int consultationId)
		{
			if (!IsAuthenticated)
			{
				return false;
			}

			if (IsAuthenticatedByAccounts) //account auth users can comment on any (open) consultation
			{
				return true;
			}
			return ValidatedConsultationIds.Any(cid => cid.Equals(consultationId));
		}

		/// <summary>
		/// Determines whether the user has access to a given record (comment or answer) by OrganisationUserId. It only applies to cookie users.
		/// This is intended for use when editing or deleting comments and answers.
		/// </summary>
		/// <param name="organisationUserId"></param>
		/// <returns></returns>
		public bool IsAuthorisedByOrganisationUserId(int organisationUserId)
		{
			if (!IsAuthenticated || IsAuthenticatedByAccounts)
			{
				return false;
			}

			return ValidatedOrganisationUserIds.Any(ouid => ouid.Equals(organisationUserId));
		}
	}
}
