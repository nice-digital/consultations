﻿using System;

namespace Comments.ViewModels
{
	public class Comment : Location
    {
        public Comment() { } //only here for model binding. don't use it in code.

        public Comment(int locationId, string sourceUri, string htmlElementId, string rangeStart, int? rangeStartOffset, string rangeEnd, int? rangeEndOffset, string quote, string order, int commentId, DateTime lastModifiedDate, string lastModifiedByUserId, string commentText, int statusId, bool show, string sectionHeader, string sectionNumber) : base(locationId, sourceUri, htmlElementId, rangeStart, rangeStartOffset, rangeEnd, rangeEndOffset, quote, order, show, sectionHeader, sectionNumber)
        {
            CommentId = commentId;
            LastModifiedDate = lastModifiedDate;
            LastModifiedByUserId = lastModifiedByUserId;
            CommentText = commentText;
	        StatusId = statusId;
		}

        public Comment(Models.Location location, Models.Comment comment) : base(location.LocationId, location.SourceURI, location.HtmlElementID,  
            location.RangeStart, location.RangeStartOffset, location.RangeEnd, location.RangeEndOffset, location.Quote, location.Order, show: true, sectionHeader: location.SectionHeader, location.SectionNumber)
        {
            CommentId = comment.CommentId;
            LastModifiedDate = comment.LastModifiedDate;
            LastModifiedByUserId = comment.LastModifiedByUserId;
            CommentText = comment.CommentText;
	        if (comment.Status != null)
		        Status = new Status(comment.Status);
	        StatusId = comment.StatusId;
			if(comment.OrganisationUser != null)
				CommenterEmail = comment.OrganisationUser.EmailAddress;
		}

        public int CommentId { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedByUserId { get; set; }
        public string CommentText { get; set; }

	    public ViewModels.Status Status { get; set; }
		public int StatusId { get; set; }
		public string CommenterEmail { get; set; }

	    public void UpdateStatusFromDBModel(Models.Status status)
	    {
		    StatusId = status.StatusId;
		    Status = new Status(status);
	    }
	    
    }
}
