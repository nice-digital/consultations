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

		/// <summary>
		/// text response questions enforce the user to type something into the text box. yes/no questions don't.
		/// </summary>
        public bool TextIsMandatory => !HasBooleanAnswer;
    }
}
