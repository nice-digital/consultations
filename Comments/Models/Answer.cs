using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Answer
    {
        public string _currentUserId;
        private Answer() { } //just for EF

        public Answer(int questionId, Guid createdByUserId, string answerText, bool? answerBoolean, Question question)
        {
            QuestionId = questionId;
            CreatedByUserId = createdByUserId;
            AnswerText = answerText ?? throw new ArgumentNullException(nameof(answerText));
            AnswerBoolean = answerBoolean;
            Question = question;
        }
    }
}
