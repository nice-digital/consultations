namespace Comments.ViewModels
{
    public class Location
    {
        public Location() { } //only here for model binding. don't use it in code.
        public Location(int locationId, int consultationId, int? documentId, string chapterSlug, string sectionSlug, string rangeStart, int? rangeStartOffset, string rangeEnd, int? rangeEndOffset, string quote)
        {
            LocationId = locationId;
            ConsultationId = consultationId;
            DocumentId = documentId;
            ChapterSlug = chapterSlug;
            SectionSlug = sectionSlug;
            RangeStart = rangeStart;
            RangeStartOffset = rangeStartOffset;
            RangeEnd = rangeEnd;
            RangeEndOffset = rangeEndOffset;
            Quote = quote;
        }

        public int LocationId { get; set; }
        public int ConsultationId { get; set; }
        public int? DocumentId { get; set; }
        public string ChapterSlug { get; set; }
        public string SectionSlug { get; set; }
        public string RangeStart { get; set; }
        public int? RangeStartOffset { get; set; }
        public string RangeEnd { get; set; }
        public int? RangeEndOffset { get; set; }
        public string Quote { get; set; }
    }
}