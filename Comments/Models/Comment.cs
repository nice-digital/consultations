using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Comment
    {
        private Comment() {} //Just for EF

        public Comment(int locationId, Guid userId, string commentText, DateTime lastModifiedDate,
            Location location)
        {
            LocationId = locationId;
            UserId = userId;
            CommentText = commentText ?? throw new ArgumentNullException(nameof(commentText));
            LastModifiedDate = lastModifiedDate;
            Location = location;
        }
    }
}
