using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comments.Models
{
    public partial class Submission
    {
	    private Submission() { } //just for EF 
		public Submission(Guid submissionByUserId, DateTime submissioDateTime)
	    {
		    SubmissionByUserId = submissionByUserId;
		    SubmissionDateTime = submissioDateTime;

	    }
    }
}
