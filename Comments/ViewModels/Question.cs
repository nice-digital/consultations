using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Edm.Expressions;

namespace Comments.ViewModels
{
    public class Question : Location
    {
        public Question() { } //only here for model binding. don't use it in code.
        public Question(Models.Location location, Models.Question question) : base(location.LocationId, location.SourceURI, location.HtmlElementID,
            location.RangeStart, location.RangeStartOffset, location.RangeEnd, location.RangeEndOffset, location.Quote, show: true)
        {
            QuestionId = question.QuestionId;
            QuestionText = question.QuestionText;
            QuestionTypeId = question.QuestionTypeId;
            QuestionOrder = question.QuestionOrder;
            LastModifiedByUserId = question.LastModifiedByUserId;
            LastModifiedDate = question.LastModifiedDate;

            QuestionType = new QuestionType(question.QuestionType);
            if (!(question.Answer is null))
                Answers = question.Answer.Select(answer => new Answer(answer)).ToList();
        }

        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int QuestionTypeId { get; set; }
        public byte? QuestionOrder { get; set; }
        public Guid LastModifiedByUserId { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public ViewModels.QuestionType QuestionType { get; set; }
        public IList<ViewModels.Answer> Answers { get; set; }
    }

    public class QuestionType
    {
        public QuestionType() { } //only here for model binding. don't use it in code.
        public QuestionType(Models.QuestionType questionType)
        {
            Description = questionType.Description;
            HasTextAnswer = questionType.HasTextAnswer;
            HasBooleanAnswer = questionType.HasBooleanAnswer;
        }

        public string Description { get; set; }
        public bool HasTextAnswer { get; set; }
        public bool HasBooleanAnswer { get; set; }
    }

    
}
