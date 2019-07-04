using System.Collections.Generic;
using System.Linq;

namespace Comments.ViewModels
{
	/// <summary>
	/// This view model should only be used on admin screens, not on the user side, as we don't really want them to have access to the sentiment analysis.
	/// </summary>
	public class CommentWithAnalysis : Comment
	{
		public CommentWithAnalysis(Models.Location location, Models.Comment comment) : base(location, comment)
		{
			Sentiment = comment.Sentiment;
			SentimentScorePositive = comment.SentimentScorePositive;
			SentimentScoreNegative = comment.SentimentScoreNegative;
			SentimentScoreNeutral = comment.SentimentScoreNeutral;
			SentimentScoreMixed = comment.SentimentScoreMixed;

			if (comment.CommentKeyPhrase != null)
			{
				KeyPhrases = comment.CommentKeyPhrase.Select(commentKeyPhrase => new ViewModels.KeyPhrase(commentKeyPhrase.KeyPhraseId, commentKeyPhrase.KeyPhrase.Text, commentKeyPhrase.Score));
			}
		}

		public string Sentiment { get; private set; }
		public float SentimentScorePositive { get; private set; }
		public float SentimentScoreNegative { get; private set; }
		public float SentimentScoreNeutral { get; private set; }
		public float SentimentScoreMixed { get; private set; }

		public IEnumerable<ViewModels.KeyPhrase> KeyPhrases { get; private set; } = new List<KeyPhrase>();
	}
}
