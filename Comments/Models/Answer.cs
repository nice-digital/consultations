using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Answer
    {
	    private Answer() //just for EF
		{
			ChildAnswers = new HashSet<Answer>();
			SubmissionAnswer = new HashSet<SubmissionAnswer>();
		}

		public Answer(int questionId, string createdByUserId, string answerText, bool? answerBoolean, Question question, int statusId, Status status, int? organisationUserId = null, int? parentAnswerId = null, int? organisationId = null)
        {
	        if (answerText == null && question.QuestionType.TextIsMandatory)
	        {
		        throw new ArgumentNullException(nameof(answerText));
	        }

	        QuestionId = questionId;
            CreatedByUserId = createdByUserId;
	        CreatedDate = DateTime.UtcNow;
	        LastModifiedDate = DateTime.UtcNow;
			AnswerText = answerText;
            AnswerBoolean = answerBoolean;
            Question = question;
	        StatusId = statusId;
	        Status = status;
	        SubmissionAnswer = new HashSet<SubmissionAnswer>();
	        OrganisationUserId = organisationUserId;
	        ParentAnswerId = parentAnswerId;
	        OrganisationId = organisationId;
        }

        public void UpdateFromViewModel(ViewModels.Answer answer)
        {
	        if (answer.AnswerText == null && Question.QuestionType.TextIsMandatory)
	        {
		        throw new ArgumentNullException(nameof(answer.AnswerText));
	        }

			LastModifiedByUserId = answer.LastModifiedByUserId;
	        LastModifiedDate = answer.LastModifiedDate;
            AnswerText = answer.AnswerText; 
            AnswerBoolean = answer.AnswerBoolean;
	        StatusId = answer.StatusId;
			//Status.UpdateFromViewModel(answer.Status);
        }

        public UserType AnswerByUserType
        {
	        get
	        {
		        if (OrganisationUserId.HasValue && !ParentAnswerId.HasValue)
		        {
			        return UserType.OrganisationalCommenter;
		        }
		        if (ParentAnswerId.HasValue || (!string.IsNullOrEmpty(CreatedByUserId) && OrganisationId.HasValue))
		        {
			        return UserType.OrganisationLead;
		        }
		        return UserType.IndividualCommenter;
	        }
        }
	}
}
