using System;
using System.Linq;
using System.Text.RegularExpressions;
using Comments.Common;

namespace Comments.ViewModels
{
    public class Location
    {
        public Location() { } //only here for model binding. don't use it in code.

        public Location(int locationId, string sourceUri, string htmlElementId, string rangeStart, int? rangeStartOffset, string rangeEnd, int? rangeEndOffset, string quote, string order, bool show, string section)
        {
            LocationId = locationId;
            SourceURI = sourceUri;
            HtmlElementID = htmlElementId;
            RangeStart = rangeStart;
            RangeStartOffset = rangeStartOffset;
            RangeEnd = rangeEnd;
            RangeEndOffset = rangeEndOffset;
            Quote = quote;
	        Order = order;
	        Show = show;
	        Section = section;
        }
        public int LocationId { get; set; }

        public string SourceURI { get; set; }
        public string HtmlElementID { get; set; }

        public string RangeStart { get; set; }
        public int? RangeStartOffset { get; set; }
        public string RangeEnd { get; set; }
        public int? RangeEndOffset { get; set; }
        public string Quote { get; set; }
		

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

	    private const int UnsetDocumentIdValue = 0;
	    private int? _documentId = UnsetDocumentIdValue;
	    
	    public int? DocumentId
	    {
		    get
		    {
			    if (_documentId == UnsetDocumentIdValue)
			    {
				    var sourceUri = SourceURI;
				    if (!ConsultationsUri.IsValidSourceURI(sourceUri))
				    {
					    sourceUri = ConsultationsUri.ConvertToConsultationsUri(SourceURI, _commentOn.Value);
				    }
				    _documentId = ConsultationsUri.ParseConsultationsUri(sourceUri).DocumentId;
			    }
			    return _documentId;
		    }
		    set => _documentId = value;
	    }

		/// <summary>
		/// When false, on the review page, the comment is not shown, i.e. it's filtered out by the filtering on that page.
		/// on the document page, it should always be true and is unused.
		/// </summary>
	    public bool Show { get; set; }

	    private string _order = null;
		/// <summary>
		/// Order is: "[consultation id].[document id].[chapter index].[html element position within chapter]", 
		/// it's a dotted decimal a bit like "002.001.000.000.002.001.001.000.000.001.002.167" zeros are padded based on the longest decimal value between dots.
		/// so after setting this field can just be ordered like a string.
		/// </summary>
		public string Order
	    {
		    get => _order ?? "0";
			set
			{
				if (!string.IsNullOrWhiteSpace(value))
				{
					var parts = value.Split('.');
					var maxDigitsPerPart =
						parts.SelectMany(i => Regex.Matches(i, @"\d+").Cast<Match>().Select(m => (int?) m.Value.Length)).Max() ?? 0;
					var partsWithPaddedZeros =
						parts.Select(i => Regex.Replace(i, @"\d+", m => m.Value.PadLeft(maxDigitsPerPart, '0')));
					_order = string.Join('.', partsWithPaddedZeros);
				}
			}
		}

	    /// <summary>
	    /// Nearest section that this comment/question belongs to. might well be null. it might also start with a dotted decimal number or not.
	    /// </summary>
	    public string Section { get; set; }
	}
}
