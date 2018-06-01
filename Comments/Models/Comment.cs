using System;

namespace Comments.Models
{
    public partial class Comment
    {
        private Comment() {} //Just for EF

        public Comment(int locationId, Guid createdByUserId, string commentText, Guid lastModifiedByUserId, Location location, int statusId, Status status)
        {
            LocationId = locationId;
            CreatedByUserId = createdByUserId;
            CreatedDate = DateTime.UtcNow;
            LastModifiedDate = DateTime.UtcNow;
            LastModifiedByUserId = lastModifiedByUserId;
            CommentText = commentText ?? throw new ArgumentNullException(nameof(commentText));
            Location = location;
	        StatusId = statusId;
	        Status = status;
        } 

        public Comment(ViewModels.Comment comment, Guid createdByUserId) : this(comment.LocationId, createdByUserId, comment.CommentText, comment.LastModifiedByUserId, location: null, statusId: comment.StatusId, status: null)
        { }

        public void UpdateFromViewModel(ViewModels.Comment comment)
        {
            LocationId = comment.LocationId;
            LastModifiedByUserId = comment.LastModifiedByUserId;
            LastModifiedDate = comment.LastModifiedDate;
            CommentText = comment.CommentText ?? throw new ArgumentNullException(nameof(comment.CommentText));
	        StatusId = comment.StatusId;
			Location.UpdateFromViewModel(comment as ViewModels.Location);
	        
        }
    }
}
