using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Location
    {
        public Location()
        {
            Comment = new HashSet<Comment>();
            Question = new HashSet<Question>();
        }

        public int LocationId { get; set; }
        public Guid ConsultationId { get; set; }
        public Guid? DocumentId { get; set; }
        public string ChapterSlug { get; set; }
        public string SectionSlug { get; set; }
        public string RangeStart { get; set; }
        public int? RangeStartOffset { get; set; }
        public string RangeEnd { get; set; }
        public int? RangeEndOffset { get; set; }
        public string Quote { get; set; }

        public ICollection<Comment> Comment { get; set; }
        public ICollection<Question> Question { get; set; }
    }
}
