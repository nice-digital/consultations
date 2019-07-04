using System.Collections.Generic;
using System.Linq;

namespace Comments.ViewModels
{
	/// <summary>
	/// This view model should only be used on admin screens, not on the user side, as we don't really want them to have access to the sentiment analysis.
	/// </summary>
	public class AnswerWithAnalysis : Answer
	{
		public string Sentiment { get; set; }
		public float SentimentScorePositive { get; set; }
		public float SentimentScoreNegative { get; set; }
		public float SentimentScoreNeutral { get; set; }
		public float SentimentScoreMixed { get; set; }
		public IEnumerable<ViewModels.KeyPhrase> KeyPhrases { get; private set; }

		public AnswerWithAnalysis(Models.Answer answer, Models.Question question = null) : base(answer, question)
		{
			Sentiment = answer.Sentiment;
			SentimentScorePositive = answer.SentimentScorePositive;
			SentimentScoreNegative = answer.SentimentScoreNegative;
			SentimentScoreNeutral = answer.SentimentScoreNeutral;
			SentimentScoreMixed = answer.SentimentScoreMixed;

			KeyPhrases = answer.AnswerKeyPhrase.Select(answerKeyPhrase => new ViewModels.KeyPhrase(answerKeyPhrase.KeyPhraseId, answerKeyPhrase.KeyPhrase.Text, answerKeyPhrase.Score));
		}
	}
}