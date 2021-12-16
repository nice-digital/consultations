using System;

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
			AnswerText = Uri.EscapeDataString(answer.AnswerText);
			AnswerBoolean = answer.AnswerBoolean;
			QuestionId = answer.QuestionId;
			LastModifiedDate = answer.LastModifiedDate;
			LastModifiedByUserId = answer.LastModifiedByUserId;
			StatusId = answer.StatusId;
			if (answer.Status != null)
				Status = new Status(answer.Status);

			if (question != null)
				SourceURI = question.Location.SourceURI;
			else if (answer.Question?.Location != null)
				SourceURI = answer.Question.Location.SourceURI;

			if (answer.OrganisationUser != null)
				CommenterEmail = answer.OrganisationUser.EmailAddress;
		}

		public int AnswerId { get; set; }
		public string AnswerText { get; set; }
		public bool? AnswerBoolean { get; set; }

		public int QuestionId { get; set; }

		public DateTime LastModifiedDate { get; set; }
		public string LastModifiedByUserId { get; set; }

		public int StatusId { get; set; }
		public ViewModels.Status Status { get; set; }

		/// <summary>
		/// This property doesn't match the database. it's only in a denormalised capacity to make things easier for the front-end.
		/// </summary>
		public string SourceURI { get; set; }

		public string CommenterEmail { get; set; }

        public bool showWhenFiltered { get; set; } = true;

		public void UpdateStatusFromDBModel(Models.Status status)
		{
			StatusId = status.StatusId;
			Status = new Status(status);
		}
	}
}
