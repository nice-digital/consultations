using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Location
    {
        public Location(int locationId, string sourceUrl, string htmlElementId, string rangeStart, int? rangeStartOffset, string rangeEnd, int? rangeEndOffset, string quote, ICollection<Comment> comment, ICollection<Question> question)
        {
            LocationId = locationId;
            SourceURL = sourceUrl;
            HtmlElementID = htmlElementId;
            RangeStart = rangeStart;
            RangeStartOffset = rangeStartOffset;
            RangeEnd = rangeEnd;
            RangeEndOffset = rangeEndOffset;
            Quote = quote;
            Comment = comment;
            Question = question;
        }

        public Location(ViewModels.Location location) : this(location.LocationId, location.SourceURL, location.HtmlElementID, 
            location.RangeStart, location.RangeStartOffset, location.RangeEnd, location.RangeEndOffset, location.Quote, null, null)
        {
        }

        public void UpdateFromViewModel(ViewModels.Location location)
        {
            SourceURL = location.SourceURL;
            HtmlElementID = location.HtmlElementID;
            RangeStart = location.RangeStart;
            RangeStartOffset = location.RangeStartOffset;
            RangeEnd = location.RangeEnd;
            RangeEndOffset = location.RangeEndOffset;
            Quote = location.Quote;
        }
    }
}