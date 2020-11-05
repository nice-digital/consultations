using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class OrganisationUser
    {
        public int OrganisationUserId { get; set; }
        public Guid AuthorisationSession { get; set; }
        public string EmailAddress { get; set; }

        public ICollection<Answer> Answer { get; set; }
        public ICollection<Comment> Comment { get; set; }
    }
}
