using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Comment
    {
        private Comment() {} //Just for EF

        public Comment(int locationId, Guid createdByUserId, string commentText, Location location, Guid lastModifiedByUserId)
        {
            LocationId = locationId;
            CreatedByUserId = createdByUserId;
            LastModifiedByUserId = lastModifiedByUserId;
            CommentText = commentText ?? throw new ArgumentNullException(nameof(commentText));
            Location = location;
        }
    }
}
