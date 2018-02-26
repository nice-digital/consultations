using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Answer
    {
        private Answer() { } //just for EF

        public Answer(int questionId, Guid userId, string answerText, bool? answerBoolean, DateTime lastModifiedDate, Question question)
        {
            QuestionId = questionId;
            UserId = userId;
            AnswerText = answerText ?? throw new ArgumentNullException(nameof(answerText));
            AnswerBoolean = answerBoolean;
            LastModifiedDate = lastModifiedDate;
            Question = question;
        }
    }
}
