using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comments.Models
{
    public partial class SubmissionAnswer
    {
	    private SubmissionAnswer() { } //just for EF

	    public SubmissionAnswer(int sumbmissionId, int answerId)
	    {
		    SubmissionId = sumbmissionId;
		    AnswerId = answerId;
	    }
	}
}
