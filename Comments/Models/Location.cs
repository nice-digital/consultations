using System;
using System.Collections.Generic;
using Comments.Common;
using Comments.ViewModels;

namespace Comments.Models
{
    public partial class Location
    {
		private Location() //Just for EF
		{
			Comment = new HashSet<Comment>();
			OrganisationAuthorisation = new HashSet<OrganisationAuthorisation>();
			Question = new HashSet<Question>();
		}
		public Location(string sourceUri, string htmlElementId, string rangeStart, int? rangeStartOffset, string rangeEnd, int? rangeEndOffset, string quote, string order, string sectionHeader, string sectionNumber, ICollection<Comment> comment, ICollection<Question> question)
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
	        SectionHeader = sectionHeader;
            SectionNumber = sectionNumber;
        }

        public Location(ViewModels.Location location) : this(location.SourceURI, location.HtmlElementID, 
            location.RangeStart, location.RangeStartOffset, location.RangeEnd, location.RangeEndOffset, location.Quote, location.Order, location.SectionHeader, location.SectionNumber, null, null)
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
	        SectionHeader = location.SectionHeader;
            SectionNumber = location.SectionNumber;
        }
    }
}
