using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Comment
    {
        public int CommentId { get; set; }
		public int LocationId { get; set; }
        public string CreatedByUserId { get; set; }
		public string CommentText { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime LastModifiedDate { get; set; }
		public string LastModifiedByUserId { get; set; }
		public bool IsDeleted { get; set; }
		public int StatusId { get; set; }
        public int? OrganisationUserId { get; set; }
        public int? OrganisationAuthorisationId { get; set; }
        public int? ParentCommentId { get; set; }

        public Location Location { get; set; }
        public OrganisationAuthorisation OrganisationAuthorisation { get; set; }
        public OrganisationUser OrganisationUser { get; set; }
        public Comment ParentComment { get; set; }
        public Status Status { get; set; }
        public ICollection<Comment> ChildComments { get; set; }
        public ICollection<SubmissionComment> SubmissionComment { get; set; }
    }
}
