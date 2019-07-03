namespace Comments.Models
{
	public partial class AnswerKeyPhrase
	{
		public AnswerKeyPhrase() {}

		public AnswerKeyPhrase(int keyPhraseId, float score)
		{
			KeyPhraseId = keyPhraseId;
			Score = score;
		}
	}
}
