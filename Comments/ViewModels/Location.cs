namespace Comments.ViewModels
{
    public class Location
    {
        public Location() { } //only here for model binding. don't use it in code.

        public Location(int locationId, string sourceUrl, string htmlElementId, string rangeStart, int? rangeStartOffset, string rangeEnd, int? rangeEndOffset, string quote)
        {
            LocationId = locationId;
            SourceURL = sourceUrl;
            HtmlElementID = htmlElementId;
            RangeStart = rangeStart;
            RangeStartOffset = rangeStartOffset;
            RangeEnd = rangeEnd;
            RangeEndOffset = rangeEndOffset;
            Quote = quote;
        }
        public int LocationId { get; set; }

        public string SourceURL { get; set; }
        public string HtmlElementID { get; set; }

        //public int ConsultationId { get; set; }
        //public int? DocumentId { get; set; }
        //public string ChapterSlug { get; set; }
        //public string SectionSlug { get; set; }
        public string RangeStart { get; set; }
        public int? RangeStartOffset { get; set; }
        public string RangeEnd { get; set; }
        public int? RangeEndOffset { get; set; }
        public string Quote { get; set; }
    }
}