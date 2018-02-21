using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class QuestionType
    {
        public QuestionType()
        {
            Question = new HashSet<Question>();
        }

        public int QuestionTypeId { get; set; }
        public string Description { get; set; }
        public bool HasTextAnswer { get; set; }
        public bool HasBooleanAnswer { get; set; }

        public ICollection<Question> Question { get; set; }
    }
}
