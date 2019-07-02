using System;
using System.Collections;
using Comments.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Comprehend;
using Amazon.Comprehend.Model;
using Answer = Comments.ViewModels.Answer;
using Comment = Comments.ViewModels.Comment;

namespace Comments.Services
{
	public interface IAnalysisService
	{
		Task AnalyseAndUpdateDatabase(ConsultationsContext context, IList<Comment> submissionComments, IList<Answer> submissionAnswers);
	}

	public class AnalysisService : IAnalysisService
	{
		private const string LanguageCode = "en";
		private const int MaximumBatchSize = 25;
		private readonly IAmazonComprehend _amazonComprehend;

		#region Batching types
		private enum CommentOrAnswer
		{
			Comment,
			Answer
		}

		private class Batches
		{
			public Batches(IEnumerable<Batch> allBatches)
			{
				AllBatches = allBatches;
			}

			public IEnumerable<Batch> AllBatches { get; private set; } 
		}

		private class Batch
		{
			public Batch(IList<ItemForAnalysis> itemsInBatch)
			{
				ItemsInBatch = itemsInBatch;
			}

			public IList<ItemForAnalysis> ItemsInBatch { get; private set; }
		}

		private class ItemForAnalysis 
		{
			public ItemForAnalysis(CommentOrAnswer type, int id, string text)
			{
				Type = type;
				Id = id;
				Text = text;
			}

			public CommentOrAnswer Type { get; }
			public int Id { get; }
			public string Text { get;  }
		}
		#endregion Batching types

		public AnalysisService(IAmazonComprehend amazonComprehend)
		{
			_amazonComprehend = amazonComprehend;
		}

		public async Task AnalyseAndUpdateDatabase(ConsultationsContext context, IList<Comment> submissionComments,
			IList<Answer> submissionAnswers)
		{
			var batchedUpCommentsAndAnswers = BatchUp(submissionComments, submissionAnswers);

			await SentimentAnalysis(context, batchedUpCommentsAndAnswers);

			KeyPhraseAnalysis(context, batchedUpCommentsAndAnswers);
		}

		private static Batches BatchUp(IList<Comment> submissionComments, IList<Answer> submissionAnswers)
		{
			var allItems = new List<ItemForAnalysis>();

			if (submissionComments != null && submissionComments.Any())
			{
				allItems.AddRange(submissionComments.Select(comment => new ItemForAnalysis(CommentOrAnswer.Comment, comment.CommentId, comment.CommentText)));
			}
			if (submissionAnswers != null && submissionAnswers.Any())
			{
				allItems.AddRange(submissionAnswers.Select(answer => new ItemForAnalysis(CommentOrAnswer.Answer, answer.AnswerId, answer.AnswerText)));
			}

			var batchedUpItems = SplitList<ItemForAnalysis>(allItems, MaximumBatchSize);

			return new Batches(batchedUpItems.Select(batch => new Batch(batch)));
		}

		private static IEnumerable<List<T>> SplitList<T>(List<T> locations, int nSize = 30)
		{
			for (var i = 0; i < locations.Count; i += nSize)
			{
				yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
			}
		}

		private async Task SentimentAnalysis(ConsultationsContext context, Batches batches)
		{
			foreach (var batch in batches.AllBatches)
			{
				var batchDetectSentimentRequest = new BatchDetectSentimentRequest()
				{
					LanguageCode = LanguageCode,
					TextList = batch.ItemsInBatch.Select(item => item.Text).ToList()
				};

				var commentIds = batch.ItemsInBatch.Where(item => item.Type == CommentOrAnswer.Comment).Select(item => item.Id).ToList();

				var oneComment = context.Comment.FirstOrDefault(comment => comment.CommentId.Equals(45013));

				var commentsToUpdate = context.Comment.Where(comment => commentIds.Contains(comment.CommentId)).ToList();

				var answerIds = batch.ItemsInBatch.Where(item => item.Type == CommentOrAnswer.Answer).Select(item => item.Id).ToList();
				var answersToUpdate = context.Answer.Where(answer => answerIds.Contains(answer.AnswerId)).ToList();

				var batchDetectSentimentResponse = await _amazonComprehend.BatchDetectSentimentAsync(batchDetectSentimentRequest);

				foreach (var result in batchDetectSentimentResponse.ResultList)
				{
					var id = batch.ItemsInBatch[result.Index].Id;

					if (batch.ItemsInBatch[result.Index].Type == CommentOrAnswer.Comment)
					{
						var commentToUpdate = commentsToUpdate.Single(comment => comment.CommentId == id);

						commentToUpdate.Sentiment = result.Sentiment.Value;
						commentToUpdate.SentimentScorePositive = result.SentimentScore.Positive;
						commentToUpdate.SentimentScoreNegative = result.SentimentScore.Negative;
						commentToUpdate.SentimentScoreNeutral = result.SentimentScore.Neutral;
						commentToUpdate.SentimentScoreMixed = result.SentimentScore.Mixed;
					}
					else
					{
						var answerToUpdate = answersToUpdate.Single(answer => answer.AnswerId == id);

						answerToUpdate.Sentiment = result.Sentiment.Value;
						answerToUpdate.SentimentScorePositive = result.SentimentScore.Positive;
						answerToUpdate.SentimentScoreNegative = result.SentimentScore.Negative;
						answerToUpdate.SentimentScoreNeutral = result.SentimentScore.Neutral;
						answerToUpdate.SentimentScoreMixed = result.SentimentScore.Mixed;
					}
				}
				context.Comment.UpdateRange(commentsToUpdate);
				context.Answer.UpdateRange(answersToUpdate);
				await context.SaveChangesAsync();
			}
		}


		private void SaveToDatabase(object sentiments, object keyPhrases)
		{
			throw new NotImplementedException();
		}

		private object KeyPhraseAnalysis(ConsultationsContext context, Batches batches)
		{
			throw new NotImplementedException();
		}
	}
}
