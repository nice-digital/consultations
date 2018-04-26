using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Answer
    {
        private Answer() { } //just for EF

        public Answer(int questionId, Guid createdByUserId, string answerText, bool? answerBoolean, Question question)
        {
            QuestionId = questionId;
            CreatedByUserId = createdByUserId;
            AnswerText = answerText ?? throw new ArgumentNullException(nameof(answerText));
            AnswerBoolean = answerBoolean;
            Question = question;
        }

        public void UpdateFromViewModel(ViewModels.Answer answer)
        {
            LastModifiedByUserId = answer.LastModifiedByUserId;
            AnswerText = answer.AnswerText ?? throw new ArgumentNullException(nameof(answer.AnswerText)); 
            AnswerBoolean = answer.AnswerBoolean;
        }
    }
}
