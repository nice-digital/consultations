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

		public Submission(Guid submissionByUserId, DateTime submissioDateTime, string organisationName, string tobaccoDisclosure)
	    {
		    SubmissionByUserId = submissionByUserId;
		    SubmissionDateTime = submissioDateTime;
		    OrganisationName = organisationName;
		    TobaccoDisclosure = tobaccoDisclosure;
		    SubmissionAnswer = new HashSet<SubmissionAnswer>();
		    SubmissionComment = new HashSet<SubmissionComment>();
		}
    }
}
