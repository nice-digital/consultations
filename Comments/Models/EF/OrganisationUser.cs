using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class OrganisationUser
    {
        public int OrganisationUserId { get; set; }
        public Guid AuthorisationSession { get; set; }
        public string EmailAddress { get; set; }
        public int OrganisationAuthorisationId { get; set; }

		public DateTime CreatedDate { get; set; }
		public DateTime ExpirationDate { get; set; }

        public OrganisationAuthorisation OrganisationAuthorisation { get; set; }
    }
}
