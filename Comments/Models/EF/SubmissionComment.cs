
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class SubmissionComment
    {
		public int SubmissionCommentId { get; set; }
		public int SubmissionId { get; set; }
		public int CommentId { get; set; }

		public Submission Submission { get; set; }

		public Comment Comment { get; set; }
	}
}
