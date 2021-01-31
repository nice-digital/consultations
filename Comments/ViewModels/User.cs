using System;
using NICE.Identity.Authentication.Sdk.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Comments.Common;
using Comments.Models;
using NICE.Identity.Authentication.Sdk.Extensions;

namespace Comments.ViewModels
{
	public class User
    {
	    public User() {}

	    public User(ClaimsPrincipal claimsPrincipal)
	    {
		    if (claimsPrincipal?.Identity == null)
		    {
			    AuthenticatedBy = AuthenticationMechanism.None;
		    }
		    else
		    {
			    AuthenticationType = claimsPrincipal.Identity?.AuthenticationType;
			    if (claimsPrincipal.Identities.Count() > 1) //it's possible to be authenticated by org cookie + idam, at the same time. 
			    {
				    AuthenticatedBy = AuthenticationMechanism.AccountsAndOrganisationCookie;
			    }
			    else
			    {
				    AuthenticatedBy = AuthenticationType == OrganisationCookieAuthenticationOptions.DefaultScheme
					    ? AuthenticationMechanism.OrganisationCookie
					    : AuthenticationMechanism.Accounts;
			    }
			    DisplayName = claimsPrincipal.DisplayName();
			    UserId = claimsPrincipal.NameIdentifier();
				OrganisationsAssignedAsLead = claimsPrincipal.OrganisationsAssignedAsLead();
				ValidatedSessions = claimsPrincipal.ValidatedSessions();
				IsAuthenticatedByAnyMechanism = claimsPrincipal?.Identity?.IsAuthenticated ?? false;
		    }
	    }

	    public User(AuthenticationMechanism authenticatedBy, string authenticationType, string displayName, string userId, IEnumerable<Organisation> organisationsAssignedAsLead, IList<ValidatedSession> validatedSessions)
	    {
		    AuthenticatedBy = authenticatedBy;
		    AuthenticationType = authenticationType;
		    IsAuthenticatedByAnyMechanism = (authenticatedBy != AuthenticationMechanism.None);
		    DisplayName = displayName;
		    UserId = userId;
		    OrganisationsAssignedAsLead = organisationsAssignedAsLead;
		    ValidatedSessions = validatedSessions;
	    }

	    public enum AuthenticationMechanism
	    {
		    None,
		    Accounts,
		    OrganisationCookie,
		    AccountsAndOrganisationCookie
	    }

		public AuthenticationMechanism AuthenticatedBy { get; private set; }

		public string AuthenticationType { get; private set; }
		public bool IsAuthenticatedByAnyMechanism { get; private set; }
        public string DisplayName { get; private set; }
        public string UserId { get; private set; }
		
		public IEnumerable<Organisation> OrganisationsAssignedAsLead { get; private set; }

		public IList<ValidatedSession> ValidatedSessions { get; private set; }

		public IList<int> ValidatedOrganisationUserIds => ValidatedSessions != null ? ValidatedSessions.Select(session => session.OrganisationUserId).ToList() : new List<int>();

		public bool IsAuthenticatedByAccounts => (IsAuthenticatedByAnyMechanism && (AuthenticatedBy == AuthenticationMechanism.Accounts || AuthenticatedBy == AuthenticationMechanism.AccountsAndOrganisationCookie));

		public bool IsAuthenticatedByOrganisationCookie => (IsAuthenticatedByAnyMechanism && (AuthenticatedBy == AuthenticationMechanism.OrganisationCookie || AuthenticatedBy == AuthenticationMechanism.AccountsAndOrganisationCookie));

		public bool IsAuthenticatedByOrganisationCookieForThisConsultation(int consultationId)
		{
			return ValidatedSessions.Any(session => session.ConsultationId.Equals(consultationId));
		}

		/// <summary>
		/// Determines whether a user is authorised to e.g. comment, on a given consultation.
		/// Organisation cookie users are only permitted to comment on the consultation they have a validated cookie for.
		/// </summary>
		/// <param name="consultationId"></param>
		/// <returns></returns>
		public bool IsAuthorisedByConsultationId(int consultationId)
		{
			if (!IsAuthenticatedByAnyMechanism)
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
			if (!IsAuthenticatedByOrganisationCookie)
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
