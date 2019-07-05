using System.Collections.Generic;
using System.Linq;

namespace Comments.ViewModels
{
	/// <summary>
	/// This view model should only be used on admin screens, not on the user side, as we don't really want them to have access to the sentiment analysis.
	/// </summary>
	public class AnswerWithAnalysis : Answer
	{
		public AnswerWithAnalysis(Models.Answer answer, Models.Question question = null) : base(answer, question)
		{
			Sentiment = answer.Sentiment;
			SentimentScorePositive = answer.SentimentScorePositive;
			SentimentScoreNegative = answer.SentimentScoreNegative;
			SentimentScoreNeutral = answer.SentimentScoreNeutral;
			SentimentScoreMixed = answer.SentimentScoreMixed;

			KeyPhrases = answer.AnswerKeyPhrase.Select(answerKeyPhrase => new ViewModels.KeyPhrase(answerKeyPhrase.KeyPhraseId, answerKeyPhrase.KeyPhrase.Text, answerKeyPhrase.Score));
		}

		public string Sentiment { get; private set; }
		public float SentimentScorePositive { get; private set; }
		public float SentimentScoreNegative { get; private set; }
		public float SentimentScoreNeutral { get; private set; }
		public float SentimentScoreMixed { get; private set; }
		public IEnumerable<ViewModels.KeyPhrase> KeyPhrases { get; private set; }
	}
}
