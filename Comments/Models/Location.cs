using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Location
    {
        public Location(Guid consultationId, Guid? documentId, string chapterSlug, string sectionSlug,
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
    }
}
