using System;
using Comments.Common;
using Microsoft.AspNetCore.Http.Extensions;

namespace Comments.ViewModels
{
    public class Comment : Location
    {
        public Comment() { } //only here for model binding. don't use it in code.

        public Comment(int locationId, string sourceUri, string htmlElementId, string rangeStart, int? rangeStartOffset, string rangeEnd, int? rangeEndOffset, string quote, int commentId, DateTime lastModifiedDate, Guid lastModifiedByUserId, string commentText, int statusId) : base(locationId, sourceUri, htmlElementId, rangeStart, rangeStartOffset, rangeEnd, rangeEndOffset, quote)
        {
            CommentId = commentId;
            LastModifiedDate = lastModifiedDate;
            LastModifiedByUserId = lastModifiedByUserId;
            CommentText = commentText;
	        Status = new Status(); statusId;
        }

        public Comment(Models.Location location, Models.Comment comment) : base(location.LocationId, location.SourceURI, location.HtmlElementID,  
            location.RangeStart, location.RangeStartOffset, location.RangeEnd, location.RangeEndOffset, location.Quote)
        {
            CommentId = comment.CommentId;
            LastModifiedDate = comment.LastModifiedDate;
            LastModifiedByUserId = comment.LastModifiedByUserId;
            CommentText = comment.CommentText;
	        Status = new Status(comment.Status);
	        StatusId = comment.Status.StatusId;
		}

        public int CommentId { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public Guid LastModifiedByUserId { get; set; }
        public string CommentText { get; set; }

	    public ViewModels.Status Status { get; set; }

		private CommentOn? _commentOn = null;
        public string CommentOn
        {
            get
            {
                if (_commentOn == null)
                {
                    _commentOn = CommentOnHelpers.GetCommentOn(SourceURI, RangeStart, HtmlElementID);
                }
                return _commentOn.HasValue ? Enum.GetName(typeof(CommentOn), _commentOn.Value) : null;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && Enum.TryParse(value, out CommentOn parsedEnum))
                {
                    _commentOn = parsedEnum;
                }
            }
        }

	    
    }
}
