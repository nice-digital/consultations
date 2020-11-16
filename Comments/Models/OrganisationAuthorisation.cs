using System;
using System.Collections.Generic;
using Comments.Common;

namespace Comments.Models
{
	public partial class OrganisationAuthorisation
	{
		private OrganisationAuthorisation() //Just for EF
		{
			Answer = new HashSet<Answer>();
			Comment = new HashSet<Comment>();
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
