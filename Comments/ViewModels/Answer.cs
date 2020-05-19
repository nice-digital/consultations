using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comments.ViewModels
{
	public class Answer
	{
		public Answer() { } //only here for model binding. don't use it in code.
		public Answer(int answerId, string answerText, bool answerBoolean, DateTime lastModifiedDate, string lastModifiedByUserId, int questionId, int statusId)
		{
			AnswerId = answerId;
			AnswerText = answerText;
			AnswerBoolean = answerBoolean;
			QuestionId = questionId;
			LastModifiedDate = lastModifiedDate;
			LastModifiedByUserId = lastModifiedByUserId;
			StatusId = statusId;
		}

		public Answer(Models.Answer answer, Models.Question question = null)
		{
			AnswerId = answer.AnswerId;
			AnswerText = answer.AnswerText;
			AnswerBoolean = answer.AnswerBoolean;
			QuestionId = answer.QuestionId;
			LastModifiedDate = answer.LastModifiedDate;
			LastModifiedByUserId = answer.LastModifiedByUserId;
			StatusId = answer.StatusId;
			if (!(answer.Status is null))
				Status = new Status(answer.Status);

			if (question != null)
				SourceURI = question.Location.SourceURI;
			else if (answer.Question?.Location != null)
				SourceURI = answer.Question.Location.SourceURI;
		}

		public int AnswerId { get; set; }
		public string AnswerText { get; set; }
		public bool? AnswerBoolean { get; set; }

		public int QuestionId { get; set; }
		//public ViewModels.Question Question { get; set; }

		public DateTime LastModifiedDate { get; set; }
		public string LastModifiedByUserId { get; set; }

		public int StatusId { get; set; }
		public ViewModels.Status Status { get; set; }

		/// <summary>
		/// This property doesn't match the database. it's only in a denormalised capacity to make things easier for the front-end.
		/// </summary>
		public string SourceURI { get; set; }

		public void UpdateStatusFromDBModel(Models.Status status)
		{
			StatusId = status.StatusId;
			Status = new Status(status);
		}
	}
}
