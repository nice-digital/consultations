using System;
using System.Collections.Generic;

namespace Comments.Models
{
	public partial class Answer
	{
		public int AnswerId { get; set; }
		public int QuestionId { get; set; }
		public Guid CreatedByUserId { get; set; }
		public string AnswerText { get; set; }
		public bool? AnswerBoolean { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime LastModifiedDate { get; set; }
		public Guid LastModifiedByUserId { get; set; }
		public bool IsDeleted { get; set; }
		public int StatusId { get; set; }

		public Question Question { get; set; }
		public Status Status { get; set; }

		public ICollection<SubmissionAnswer> SubmissionAnswer { get; set; }

		public string Sentiment { get; set; }
		public float SentimentScorePositive { get; set; }
		public float SentimentScoreNegative { get; set; }
		public float SentimentScoreNeutral { get; set; }
		public float SentimentScoreMixed { get; set; }
	}
}

