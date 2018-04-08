using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Answer
    {
        public int AnswerId { get; set; }
        public int QuestionId { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string AnswerText { get; set; }
        public bool? AnswerBoolean { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public Guid LastModifiedByUserId { get; set; }
        public bool IsDeleted { get; set; }

        public Question Question { get; set; }
    }
}
