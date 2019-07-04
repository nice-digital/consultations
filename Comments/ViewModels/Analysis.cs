//using System.Collections.Generic;
//using System.Linq;

//namespace Comments.ViewModels
//{
//	public class Analysis
//	{
//		public Analysis(IEnumerable<QuestionWithAnalysis> questions, IEnumerable<CommentWithAnalysis> comments)
//		{
//			Questions = questions;
//			Comments = comments;
//		}

//		public IEnumerable<ViewModels.QuestionWithAnalysis> Questions { get; private set; }
//		public IEnumerable<ViewModels.CommentWithAnalysis> Comments { get; private set; }
		
//		public IEnumerable<string> AllSentiments
//		{
//			get
//			{
//				var sentiments = Comments.Select(comment => comment.Sentiment).ToList();
//				foreach (var question in Questions)
//				{
//					sentiments.AddRange(question.Answers.Select(answer => answer.Sentiment).ToList());
//				}
//				return sentiments.Distinct();
//			}
//		}

//		public IEnumerable<ViewModels.KeyPhrase> AllKeyPhrases
//		{
//			get
//			{
//				var keyPhrases = Comments.SelectMany(comment => comment.KeyPhrases).ToList();
//				foreach (var question in Questions)
//				{
//					keyPhrases.AddRange(question.Answers.SelectMany(answer => answer.KeyPhrases));
//				}
//				return keyPhrases;
//			}
//		}
//	}
//}
