using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Comment
    {
        private readonly Guid? _currentUserId;
        public int CommentId { get; set; }
        public int LocationId { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string CommentText { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public Guid LastModifiedByUserId { get; set; }
        public bool IsDeleted { get; set; }

        public Location Location { get; set; }
    }
}
