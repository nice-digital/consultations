using System;
using System.Collections.Generic;

namespace Comments.Models
{
	public partial class Question
	{
		private Question()
		{
			Answer = new HashSet<Answer>();
		}

		public int QuestionId { get; set; }
		public int LocationId { get; set; }
		public string QuestionText { get; set; }
		public int QuestionTypeId { get; set; }
		public string CreatedByUserId { get; set; }
		public DateTime CreatedDate { get; set; }
		public string LastModifiedByUserId { get; set; }
		public DateTime LastModifiedDate { get; set; }
		public bool IsDeleted { get; set; }

		public Location Location { get; set; }
		public QuestionType QuestionType { get; set; }
		public ICollection<Answer> Answer { get; set; }
	}
}
