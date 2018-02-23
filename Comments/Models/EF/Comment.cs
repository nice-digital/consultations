using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Comment
    {
        public int CommentId { get; set; }
        public int LocationId { get; set; }
        public Guid UserId { get; set; }
        public string CommentText { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public Location Location { get; set; }
    }
}
