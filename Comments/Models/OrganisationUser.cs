using System;

namespace Comments.Models
{
	public partial class OrganisationUser
	{
		public OrganisationUser() //Just for EF
		{
		}

		public OrganisationUser(int organisationAuthorisationId, Guid authorisationSession, DateTime expirationDate)
		{
			OrganisationAuthorisationId = organisationAuthorisationId;
			AuthorisationSession = authorisationSession;
			ExpirationDate = expirationDate;
		}
	}
}
