using System;
using System.Collections.Generic;

namespace Comments.Models
{
	public partial class OrganisationAuthorisation
	{
		private OrganisationAuthorisation() //Just for EF
		{
			OrganisationUsers = new HashSet<OrganisationUser>();
		}

		public OrganisationAuthorisation(string createdByUserId, DateTime createdDate, int organisationId, int locationId, string collationCode)
		{
			CreatedByUserId = createdByUserId;
			CreatedDate = createdDate;
			OrganisationId = organisationId;
			LocationId = locationId;
			CollationCode = collationCode;
		}
	}
}
