using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comments.Models
{
    public partial class SubmissionComment
    {
	    private SubmissionComment() { } //just for EF

		public SubmissionComment(int sumbmissionId, int commentId)
		{
			SubmissionId = sumbmissionId;
			CommentId = commentId;
		}
	}
}
