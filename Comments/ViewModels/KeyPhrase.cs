namespace Comments.ViewModels
{
	public class KeyPhrase
	{
		public KeyPhrase(int keyPhraseId, string text, float score)
		{
			KeyPhraseId = keyPhraseId;
			Text = text;
			Score = score;
		}

		public int KeyPhraseId { get; private set; }
		public string Text { get; private set; }
		public float Score { get; private set; }
	}
}