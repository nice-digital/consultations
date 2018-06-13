namespace Comments.Models.EF
{
    public partial class SubmissionAnswer
    {
		public int SubmissionAnswerId { get; set; }
		public int SubmissionId { get; set; }
		public int AnswerId { get; set; }

	    public Submission Submission { get; set; }
	    public Answer Answer { get; set; }
	}
}
