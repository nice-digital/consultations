using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Submission
    {
		public int SubmissionId { get; set; }
		public DateTime SubmissionDateTime { get; set; }
		public string SubmissionByUserId { get; set; }

	    public bool RespondingAsOrganisation { get; set; }
		public string OrganisationName { get; set; }

		public bool HasTobaccoLinks { get; set; }
		public string TobaccoDisclosure { get; set; }

		public bool? OrganisationExpressionOfInterest { get; set; }

		public ICollection<SubmissionComment> SubmissionComment { get; set; }
	    public ICollection<SubmissionAnswer> SubmissionAnswer { get; set; }
	}
}
