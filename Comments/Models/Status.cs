using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Status
    {
        public Status(string name, ICollection<Comment> comment, ICollection<Answer> answer)
        {
	        Name = name;
	        Comment = comment;
	        Answer = answer;
        }
	}
}
