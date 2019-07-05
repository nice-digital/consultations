namespace Comments.Models
{
	public partial class AnswerKeyPhrase
	{
		public AnswerKeyPhrase() {}

		public AnswerKeyPhrase(int answerId, int keyPhraseId, float score)
		{
			AnswerId = answerId;
			KeyPhraseId = keyPhraseId;
			Score = score;
		}
	}
}
