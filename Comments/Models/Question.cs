using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Question
    {
        public Question(int locationId, string questionText, int questionTypeId, byte? questionOrder, Location location,
            QuestionType questionType, ICollection<Answer> answer)
        {
            LocationId = locationId;
            QuestionText = questionText ?? throw new ArgumentNullException(nameof(questionText));
            QuestionTypeId = questionTypeId;
            QuestionOrder = questionOrder;
            Location = location;
            QuestionType = questionType;
            Answer = answer;
        }
    }
}
