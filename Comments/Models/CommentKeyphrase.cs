namespace Comments.Models
{
	public partial class CommentKeyPhrase
	{
		public CommentKeyPhrase() {}

		public CommentKeyPhrase(int keyPhraseId, float score)
		{
			KeyPhraseId = keyPhraseId;
			Score = score;
		}
	}
}
