using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Status
    {
        public Status()
        {
            Answer = new HashSet<Answer>();
            Comment = new HashSet<Comment>();
        }

        public int StatusId { get; set; }
        public string Name { get; set; }

        public ICollection<Answer> Answer { get; set; }
        public ICollection<Comment> Comment { get; set; }
    }
}
