using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Submission
    {
		public int SubmissionId { get; set; }
		public DateTime SubmissionDateTime { get; set; }
		public Guid SubmissionByUserId { get; set; }

		public string OrganisationName { get; set; }
	    public string TobaccoDisclosure { get; set; }

		public ICollection<SubmissionComment> SubmissionComment { get; set; }
	    public ICollection<SubmissionAnswer> SubmissionAnswer { get; set; }
	}
}
