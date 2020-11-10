using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Answer
    {
        public int AnswerId { get; set; }
		public int QuestionId { get; set; }
		public string CreatedByUserId { get; set; }
		public string AnswerText { get; set; }
		public bool? AnswerBoolean { get; set; }
        public DateTime CreatedDate { get; set; }
		public DateTime LastModifiedDate { get; set; }
		public string LastModifiedByUserId { get; set; }
		public int StatusId { get; set; }
        public int? OrganisationUserId { get; set; }
        public int? OrganisationAuthorisationId { get; set; }
        public int? ParentAnswerId { get; set; }

        public OrganisationAuthorisation OrganisationAuthorisation { get; set; }
        public OrganisationUser OrganisationUser { get; set; }
        public Answer ParentAnswer { get; set; }
        public Question Question { get; set; }
        public Status Status { get; set; }
        public ICollection<Answer> ChildAnswers { get; set; }
        public ICollection<SubmissionAnswer> SubmissionAnswer { get; set; }
    }
}
