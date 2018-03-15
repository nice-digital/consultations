using System;
using System.Collections.Generic;
using System.Linq;

namespace Comments.ViewModels
{
    public class Question : Location
    {
        public Question() { } //only here for model binding. don't use it in code.
        public Question(Models.Location location, Models.Question question) : base(location.LocationId, location.SourceURI, location.HtmlElementID,
            location.RangeStart, location.RangeStartOffset, location.RangeEnd, location.RangeEndOffset, location.Quote)
        {
            QuestionId = question.QuestionId;
            QuestionText = question.QuestionText;
            QuestionOrder = question.QuestionOrder;
            LastModifiedByUserId = question.LastModifiedByUserId;
            LastModifiedDate = question.LastModifiedDate;

            QuestionType = new QuestionType(question.QuestionType);
            Answers = question.Answer.Select(answer => new Answer(answer)).ToList();
        }

        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public byte? QuestionOrder { get; set; }
        public Guid LastModifiedByUserId { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public ViewModels.QuestionType QuestionType { get; set; }
        public IEnumerable<ViewModels.Answer> Answers { get; set; }
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

    public class Answer
    {
        public Answer() { } //only here for model binding. don't use it in code.
        public Answer(Models.Answer answer)
        {
            AnswerId = answer.AnswerId;
            AnswerText = answer.AnswerText;
            AnswerBoolean = answer.AnswerBoolean;
            LastModifiedDate = answer.LastModifiedDate;
            LastModifiedByUserId = answer.LastModifiedByUserId;
        }

        public int AnswerId { get; set; }
        public string AnswerText { get; set; }
        public bool? AnswerBoolean { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public Guid LastModifiedByUserId { get; set; }
    }
}