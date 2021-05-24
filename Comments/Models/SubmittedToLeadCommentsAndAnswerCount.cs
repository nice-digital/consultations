namespace Comments.Models
{
	public class SubmittedToLeadCommentsAndAnswerCount
	{
		public string SourceURI { get; set; }
        public int OrganisationId { get; set; }
		public int CommentCount { get; set; }
		public int AnswerCount { get; set; }
		public int TotalCount { get; set; }
	}
}
