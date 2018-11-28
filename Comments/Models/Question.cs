using System;
using System.Collections.Generic;

namespace Comments.Models
{
    public partial class Question
    {
		public Question(int locationId, string questionText, int questionTypeId, Location location,
            QuestionType questionType, ICollection<Answer> answer)
        {
            LocationId = locationId;
            QuestionText = questionText ?? throw new ArgumentNullException(nameof(questionText));
            QuestionTypeId = questionTypeId;
            Location = location;
            QuestionType = questionType;
            Answer = answer;
	        Answer = new HashSet<Answer>();
		}

        public void UpdateFromViewModel(ViewModels.Question question)
        {
            LastModifiedByUserId = question.LastModifiedByUserId;
	        LastModifiedDate = question.LastModifiedDate;
            QuestionText = question.QuestionText ?? throw new ArgumentNullException(nameof(question.QuestionText));
            Location.UpdateFromViewModel(question as ViewModels.Location);
        }
    }
}
