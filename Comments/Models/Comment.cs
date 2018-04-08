using System;

namespace Comments.Models
{
    public partial class Comment
    {
        private string _currentUserId;
        private Comment() {} //Just for EF

        public Comment(int locationId, Guid createdByUserId, string commentText, Guid lastModifiedByUserId, Location location)
        {
            LocationId = locationId;
            CreatedByUserId = createdByUserId;
            LastModifiedByUserId = lastModifiedByUserId;
            CommentText = commentText ?? throw new ArgumentNullException(nameof(commentText));
            Location = location;
        }

        public Comment(ViewModels.Comment comment, Guid createdByUserId) : this(comment.LocationId, createdByUserId, comment.CommentText, comment.LastModifiedByUserId, location: null)
        { }

        public void UpdateFromViewModel(ViewModels.Comment comment)
        {
            LocationId = comment.LocationId;
            LastModifiedByUserId = comment.LastModifiedByUserId;
            CommentText = comment.CommentText ?? throw new ArgumentNullException(nameof(comment.CommentText));
            Location.UpdateFromViewModel(comment as ViewModels.Location);
        }
    }
}
