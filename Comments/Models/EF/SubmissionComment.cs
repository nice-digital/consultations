
namespace Comments.Models.EF
{
    public partial class SubmissionComment
    {
		public int SubmissionCommentId { get; set; }
		public int SubmissionId { get; set; }
		public int CommentId { get; set; }
    }
}
