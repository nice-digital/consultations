using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class OrganisationAuthorisation
    {
        public int OrganisationAuthorisationId { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int OrganisationId { get; set; }
        public int LocationId { get; set; }
        public int CollationCode { get; set; }

        public Location Location { get; set; }
        public ICollection<Answer> Answer { get; set; }
        public ICollection<Comment> Comment { get; set; }
    }
}
