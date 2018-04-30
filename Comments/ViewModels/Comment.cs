using System;

namespace Comments.ViewModels
{
    public class Comment : Location
    {
        public Comment() { } //only here for model binding. don't use it in code.

        public Comment(int locationId, string sourceUri, string htmlElementId, string rangeStart, int? rangeStartOffset, string rangeEnd, int? rangeEndOffset, string quote, int commentId, DateTime lastModifiedDate, Guid lastModifiedByUserId, string commentText) : base(locationId, sourceUri, htmlElementId, rangeStart, rangeStartOffset, rangeEnd, rangeEndOffset, quote)
        {
            CommentId = commentId;
            LastModifiedDate = lastModifiedDate;
            LastModifiedByUserId = lastModifiedByUserId;
            CommentText = commentText;
        }

        public Comment(Models.Location location, Models.Comment comment) : base(location.LocationId, location.SourceURI, location.HtmlElementID,  
            location.RangeStart, location.RangeStartOffset, location.RangeEnd, location.RangeEndOffset, location.Quote)
        {
            CommentId = comment.CommentId;
            LastModifiedDate = comment.LastModifiedDate;
            LastModifiedByUserId = comment.LastModifiedByUserId;
            CommentText = comment.CommentText;
        }

        public int CommentId { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public Guid LastModifiedByUserId { get; set; }
        public string CommentText { get; set; }

        private string _commentOn = null;
        public string CommentOn
        {
            get
            {
                if (_commentOn == null)
                {
                    _commentOn = Common.UriHelpers.GetCommentOn(SourceURI, HtmlElementID, RangeStart);
                }
                return _commentOn;
            }
            set => _commentOn = value;
        }
    }
}