using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Answer
    {
        public int AnswerId { get; set; }
        public int QuestionId { get; set; }
        public Guid UserId { get; set; }
        public string AnswerText { get; set; }
        public bool? AnswerBoolean { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public Question Question { get; set; }
    }
}
