using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Comment
    {
	    private Comment() //Just for EF
		{
			ChildComments = new HashSet<Comment>();
			SubmissionComment = new HashSet<SubmissionComment>();
		} 

        public Comment(int locationId, string createdByUserId, string commentText, string lastModifiedByUserId, Location location, int statusId, Status status, int? organisationUserId = null, int? parentCommentId = null, int? organisationId = null)
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
	        SubmissionComment = new HashSet<SubmissionComment>();
	        OrganisationUserId = organisationUserId;
	        ParentCommentId = parentCommentId;
	        OrganisationId = organisationId;
        }

	    public Comment(ViewModels.Comment comment, string createdByUserId) : this(comment.LocationId, createdByUserId,
		    comment.CommentText, comment.LastModifiedByUserId, location: null, statusId: comment.StatusId, status: null, organisationUserId: null, parentCommentId: null, organisationId: null)
	    {
			SubmissionComment = new HashSet<SubmissionComment>();
		}

        public void UpdateFromViewModel(ViewModels.Comment comment)
        {
            LocationId = comment.LocationId;
            LastModifiedByUserId = comment.LastModifiedByUserId;
            LastModifiedDate = comment.LastModifiedDate;
            CommentText = comment.CommentText ?? throw new ArgumentNullException(nameof(comment.CommentText));
	        StatusId = comment.StatusId;
	        Location?.UpdateFromViewModel(comment as ViewModels.Location);
	        //Status.UpdateFromViewModel(comment.Status);
        }

        public UserType CommentByUserType
        {
	        get
	        {
		        if (OrganisationUserId.HasValue && !ParentCommentId.HasValue)
		        {
			        return UserType.OrganisationalCommenter;
		        }
		        if (ParentCommentId.HasValue || (!string.IsNullOrEmpty(CreatedByUserId) && OrganisationId.HasValue))
		        {
			        return UserType.OrganisationLead;
		        }
		        return UserType.IndividualCommenter;
	        }
        }
    }
}
