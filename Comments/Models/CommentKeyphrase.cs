namespace Comments.Models
{
	public partial class CommentKeyPhrase
	{
		public CommentKeyPhrase() {}

		public CommentKeyPhrase(int commentId, int keyPhraseId, float score)
		{
			CommentId = commentId;
			KeyPhraseId = keyPhraseId;
			Score = score;
		}
	}
}
