using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Location
    {
        public Location(string sourceUri, string htmlElementId, string rangeStart, int? rangeStartOffset, string rangeEnd, int? rangeEndOffset, string quote, string order, ICollection<Comment> comment, ICollection<Question> question)
        {
            SourceURI = sourceUri;
            HtmlElementID = htmlElementId;
            RangeStart = rangeStart;
            RangeStartOffset = rangeStartOffset;
            RangeEnd = rangeEnd;
            RangeEndOffset = rangeEndOffset;
            Quote = quote;
	        Order = order;
			Comment = comment;
            Question = question;
        }

        public Location(ViewModels.Location location) : this(location.SourceURI, location.HtmlElementID, 
            location.RangeStart, location.RangeStartOffset, location.RangeEnd, location.RangeEndOffset, location.Quote, location.Order, null, null)
        {
        }

        public void UpdateFromViewModel(ViewModels.Location location)
        {
            SourceURI = location.SourceURI;
            HtmlElementID = location.HtmlElementID;
            RangeStart = location.RangeStart;
            RangeStartOffset = location.RangeStartOffset;
            RangeEnd = location.RangeEnd;
            RangeEndOffset = location.RangeEndOffset;
            Quote = location.Quote;
	        Order = location.Order;
        }
    }
}
