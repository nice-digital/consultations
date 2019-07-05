namespace Comments.Models
{
	public class SubmittedCommentsAndAnswerCount
	{
		public string SourceURI { get; set; }
		public int CommentCount { get; set; }
		public int AnswerCount { get; set; }
		public int TotalCount { get; set; }
		public bool Analysed { get; set; }
	}
}
