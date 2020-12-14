using System;
using NICE.Identity.Authentication.Sdk.Domain;
using System.Collections.Generic;
using System.Linq;
using Comments.Models;

namespace Comments.ViewModels
{
	public class User
    {
	    public User() {}

	    public User(bool isAuthenticated, string displayName, string userId, IEnumerable<Organisation> organisationsAssignedAsLead, IList<ValidatedSession> validatedSessions)
		{
            IsAuthenticated = isAuthenticated;
            DisplayName = displayName;
            UserId = userId;
	        OrganisationsAssignedAsLead = organisationsAssignedAsLead;
	        ValidatedSessions = validatedSessions;
		}

        public bool IsAuthenticated { get; private set; }
        public string DisplayName { get; private set; }
        public string UserId { get; private set; }
		
		public IEnumerable<Organisation> OrganisationsAssignedAsLead { get; private set; }

		public IList<ValidatedSession> ValidatedSessions { get; private set; }

		public IList<int> ValidatedOrganisationUserIds => ValidatedSessions != null ? ValidatedSessions.Select(session => session.OrganisationUserId).ToList() : new List<int>();

		public bool IsAuthenticatedByAccounts => (IsAuthenticated && UserId != null);
		public bool IsAuthenticatedByOrganisationCookie => (IsAuthenticated && UserId == null && ValidatedSessions.Any());

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
			return ValidatedSessions.Any(session => session.ConsultationId.Equals(consultationId));
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

			return ValidatedSessions.Any(session => session.OrganisationUserId.Equals(organisationUserId));
		}

		public Guid? GetValidatedSessionIdForConsultation(int consultationId)
		{
			if (!IsAuthorisedByConsultationId(consultationId))
			{
				return null;
			}

			return ValidatedSessions.FirstOrDefault(session => session.ConsultationId.Equals(consultationId))?.SessionId;
		}
	}
}
