using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comments.Models
{
    public partial class Submission
    {
	    private Submission() //just for EF 
	    {
		    SubmissionAnswer = new HashSet<SubmissionAnswer>();
		    SubmissionComment = new HashSet<SubmissionComment>();
		}

		public Submission(Guid submissionByUserId, DateTime submissioDateTime, bool respondingAsOrganisation, string organisationName, bool hasTobaccoLinks, string tobaccoDisclosure, bool? organisationExpressionOfInterest)
	    {
		    SubmissionByUserId = submissionByUserId;
		    SubmissionDateTime = submissioDateTime;
		    RespondingAsOrganisation = respondingAsOrganisation;
		    OrganisationName = organisationName;
		    HasTobaccoLinks = hasTobaccoLinks;
		    TobaccoDisclosure = tobaccoDisclosure;
		    SubmissionAnswer = new HashSet<SubmissionAnswer>();
		    SubmissionComment = new HashSet<SubmissionComment>();
		    OrganisationExpressionOfInterest = organisationExpressionOfInterest;
	    }
    }
}
