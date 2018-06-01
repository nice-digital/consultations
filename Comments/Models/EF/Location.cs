using System;
using System.Collections.Generic;

namespace Comments.Models
{
	public partial class Location
	{
		private Location()
		{
			Comment = new HashSet<Comment>();
			Question = new HashSet<Question>();
		}

		public int LocationId { get; set; }
		public string SourceURI { get; set; }
		public string HtmlElementID { get; set; }
		public string RangeStart { get; set; }
		public int? RangeStartOffset { get; set; }
		public string RangeEnd { get; set; }
		public int? RangeEndOffset { get; set; }
		public string Quote { get; set; }

		public ICollection<Comment> Comment { get; set; }
		public ICollection<Question> Question { get; set; }
	}
}
