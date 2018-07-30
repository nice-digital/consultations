using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Answer
    {
	    private Answer() //just for EF
		{
			SubmissionAnswer = new HashSet<SubmissionAnswer>();
		} 

        public Answer(int questionId, Guid createdByUserId, string answerText, bool? answerBoolean, Question question, int statusId, Status status)
        {
            QuestionId = questionId;
            CreatedByUserId = createdByUserId;
	        CreatedDate = DateTime.UtcNow;
	        LastModifiedDate = DateTime.UtcNow;
			AnswerText = answerText ?? throw new ArgumentNullException(nameof(answerText));
            AnswerBoolean = answerBoolean;
            Question = question;
	        StatusId = statusId;
	        Status = status;
	        SubmissionAnswer = new HashSet<SubmissionAnswer>();
		}

        public void UpdateFromViewModel(ViewModels.Answer answer)
        {
            LastModifiedByUserId = answer.LastModifiedByUserId;
	        LastModifiedDate = answer.LastModifiedDate;
            AnswerText = answer.AnswerText ?? throw new ArgumentNullException(nameof(answer.AnswerText)); 
            AnswerBoolean = answer.AnswerBoolean;
	        StatusId = answer.StatusId;
			//Status.UpdateFromViewModel(answer.Status);
        }
    }
}
