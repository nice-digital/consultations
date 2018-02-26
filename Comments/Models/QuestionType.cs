using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class QuestionType
    {
        public QuestionType(string description, bool hasTextAnswer, bool hasBooleanAnswer,
            ICollection<Question> question)
        {
            Description = description ?? throw new ArgumentNullException(nameof(description));
            HasTextAnswer = hasTextAnswer;
            HasBooleanAnswer = hasBooleanAnswer;
            Question = question;
        }
    }
}
