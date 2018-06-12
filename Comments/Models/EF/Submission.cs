using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comments.Models.EF
{
    public partial class Submission
    {
		public int SubmissionId { get; set; }
		public DateTime SubmissionDateTime { get; set; }
		public Guid SubmissionByUserId { get; set; }

    }
}
