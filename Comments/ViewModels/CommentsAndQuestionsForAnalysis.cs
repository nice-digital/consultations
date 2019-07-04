using System.Collections.Generic;
using System.Linq;

namespace Comments.ViewModels
{
	public class CommentsAndQuestionsForAnalysis : CommentsAndQuestions
	{
		public CommentsAndQuestionsForAnalysis(List<Comment> comments, List<Question> questions, bool isAuthorised, string signInURL, ConsultationState consultationState,
			IEnumerable<QuestionWithAnalysis> questionsWithAnalysis, IEnumerable<CommentWithAnalysis> commentsWithAnalysis,
			string consultationTitle)
			: base(comments, questions, isAuthorised, signInURL, consultationState)
		{
			Questions = questionsWithAnalysis;
			Comments = commentsWithAnalysis;
			ConsultationTitle = consultationTitle;
		}

		public new IEnumerable<ViewModels.QuestionWithAnalysis> Questions { get; private set; }
		public new IEnumerable<ViewModels.CommentWithAnalysis> Comments { get; private set; }

		public string ConsultationTitle { get; set; }

		public IEnumerable<string> AllSentiments
		{
			get
			{
				var sentiments = Comments.Select(comment => comment.Sentiment).ToList();
				foreach (var question in Questions)
				{
					sentiments.AddRange(question.Answers.Select(answer => answer.Sentiment).ToList());
				}
				return sentiments;
			}
		}

		public IEnumerable<ViewModels.KeyPhrase> AllKeyPhrases
		{
			get
			{
				var keyPhrases = Comments.SelectMany(comment => comment.KeyPhrases).ToList();
				foreach (var question in Questions)
				{
					keyPhrases.AddRange(question.Answers.SelectMany(answer => answer.KeyPhrases));
				}
				return keyPhrases;
			}
		}
	}
}
