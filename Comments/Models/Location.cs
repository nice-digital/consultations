using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Location
    {
        public Location(int consultationId, int? documentId, string chapterSlug, string sectionSlug,
            string rangeStart, int? rangeStartOffset, string rangeEnd, int? rangeEndOffset, string quote,
            ICollection<Comment> comment, ICollection<Question> question)
        {
            ConsultationId = consultationId;
            DocumentId = documentId;
            ChapterSlug = chapterSlug;
            SectionSlug = sectionSlug;
            RangeStart = rangeStart;
            RangeStartOffset = rangeStartOffset;
            RangeEnd = rangeEnd;
            RangeEndOffset = rangeEndOffset;
            Quote = quote;
            Comment = comment;
            Question = question;
        }

        public Location(ViewModels.Location location) : this(location.ConsultationId, location.DocumentId, location.ChapterSlug, location.SectionSlug, 
            location.RangeStart, location.RangeStartOffset, location.RangeEnd, location.RangeEndOffset, location.Quote, null, null)
        {
        }

        public void UpdateFromViewModel(ViewModels.Location location)
        {
            ConsultationId = location.ConsultationId;
            DocumentId = location.DocumentId;
            ChapterSlug = location.ChapterSlug;
            SectionSlug = location.SectionSlug;
            RangeStart = location.RangeStart;
            RangeStartOffset = location.RangeStartOffset;
            RangeEnd = location.RangeEnd;
            RangeEndOffset = location.RangeEndOffset;
            Quote = location.Quote;
        }
    }
}
