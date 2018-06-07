using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comments.ViewModels
{
	public class Answer
	{
		public Answer() { } //only here for model binding. don't use it in code.
		public Answer(int answerId, string answerText, bool answerBoolean, DateTime lastModifiedDate, Guid lastModifiedByUserId, int questionId, int statusId)
		{
			AnswerId = answerId;
			AnswerText = answerText;
			AnswerBoolean = answerBoolean;
			QuestionId = questionId;
			LastModifiedDate = lastModifiedDate;
			LastModifiedByUserId = lastModifiedByUserId;
			StatusId = statusId;
		}

		public Answer(Models.Answer answer)
		{
			AnswerId = answer.AnswerId;
			AnswerText = answer.AnswerText;
			AnswerBoolean = answer.AnswerBoolean;
			QuestionId = answer.QuestionId;
			LastModifiedDate = answer.LastModifiedDate;
			LastModifiedByUserId = answer.LastModifiedByUserId;
			StatusId = 1; //answer.StatusId;
			Status = null; //new Status(answer.Status);
		}

		public int AnswerId { get; set; }
		public string AnswerText { get; set; }
		public bool? AnswerBoolean { get; set; }

		public int QuestionId { get; set; }
		public DateTime LastModifiedDate { get; set; }
		public Guid LastModifiedByUserId { get; set; }

		public int StatusId { get; set; }
		public ViewModels.Status Status { get; set; }
	}
}
