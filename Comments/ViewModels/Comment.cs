using System;

namespace Comments.ViewModels
{
    public class Comment : Location
    {
        public Comment() { } //only here for model binding. don't use it in code.

        public Comment(int locationId, int consultationId, int? documentId, string chapterSlug, string sectionSlug, string rangeStart, int? rangeStartOffset, string rangeEnd, int? rangeEndOffset, string quote, int commentId, DateTime lastModifiedDate, Guid lastModifiedByUserId, string commentText) : base(locationId, consultationId, documentId, chapterSlug, sectionSlug, rangeStart, rangeStartOffset, rangeEnd, rangeEndOffset, quote)
        {
            CommentId = commentId;
            LastModifiedDate = lastModifiedDate;
            LastModifiedByUserId = lastModifiedByUserId;
            CommentText = commentText;
        }

        public Comment(Models.Location location, Models.Comment comment) : base(location.LocationId, location.ConsultationId, location.DocumentId, location.ChapterSlug, location.SectionSlug, 
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
    }
}