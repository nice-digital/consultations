using System;

namespace Comments.Models
{
	public partial class OrganisationUser
	{
		public OrganisationUser() //Just for EF
		{
		}

		public OrganisationUser(int organisationAuthorisationId, Guid authorisationSession)
		{
			OrganisationAuthorisationId = organisationAuthorisationId;
			AuthorisationSession = authorisationSession;
		}
	}
}
