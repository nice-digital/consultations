using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Question
    {
        private Question()
        {
            Answer = new HashSet<Answer>();
        }

        public int QuestionId { get; set; }
        public int LocationId { get; set; }
        public string QuestionText { get; set; }
        public int QuestionTypeId { get; set; }
        public byte? QuestionOrder { get; set; }

        public Location Location { get; set; }
        public QuestionType QuestionType { get; set; }
        public ICollection<Answer> Answer { get; set; }
    }
}
