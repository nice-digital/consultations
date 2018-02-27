using System;
using System.Collections.Generic;
using System.Linq;

namespace Comments.ViewModels
{
    public class Location
    {
        public Location(int locationId, int consultationId, int? documentId, string chapterSlug, string sectionSlug, string rangeStart, int? rangeStartOffset, string rangeEnd, int? rangeEndOffset, string quote)
        {
            LocationId = locationId;
            ConsultationId = consultationId;
            DocumentId = documentId;
            ChapterSlug = chapterSlug;
            SectionSlug = sectionSlug;
            RangeStart = rangeStart;
            RangeStartOffset = rangeStartOffset;
            RangeEnd = rangeEnd;
            RangeEndOffset = rangeEndOffset;
            Quote = quote;
        }

        public int LocationId { get; private set; }
        public int ConsultationId { get; private set; }
        public int? DocumentId { get; private set; }
        public string ChapterSlug { get; private set; }
        public string SectionSlug { get; private set; }
        public string RangeStart { get; private set; }
        public int? RangeStartOffset { get; private set; }
        public string RangeEnd { get; private set; }
        public int? RangeEndOffset { get; private set; }
        public string Quote { get; private set; }
    }

    public class Comment : Location
    {
        public Comment(Models.Location location, Models.Comment comment) : base(location.LocationId, location.ConsultationId, location.DocumentId, location.ChapterSlug, location.SectionSlug, 
            location.RangeStart, location.RangeStartOffset, location.RangeEnd, location.RangeEndOffset, location.Quote)
        {
            CommentId = comment.CommentId;
            LastModifiedDate = comment.LastModifiedDate;
            LastModifiedByUserId = comment.LastModifiedByUserId;
            CommentText = comment.CommentText;
        }

        public int CommentId { get; private set; }
        public DateTime LastModifiedDate { get; private set; }
        public Guid LastModifiedByUserId { get; private set; }
        public string CommentText { get; private set; }
    }

    public class Question : Location
    {
        public Question(Models.Location location, Models.Question question) : base(location.LocationId, location.ConsultationId, location.DocumentId, location.ChapterSlug, location.SectionSlug,
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

        public int QuestionId { get; private set; }
        public string QuestionText { get; private set; }
        public byte? QuestionOrder { get; private set; }
        public Guid LastModifiedByUserId { get; private set; }
        public DateTime LastModifiedDate { get; private set; }

        public ViewModels.QuestionType QuestionType { get; private set; }
        public IEnumerable<ViewModels.Answer> Answers { get; private set; }
    }

    public class QuestionType
    {
        public QuestionType(Models.QuestionType questionType)
        {
            Description = questionType.Description;
            HasTextAnswer = questionType.HasTextAnswer;
            HasBooleanAnswer = questionType.HasBooleanAnswer;
        }

        public string Description { get; private set; }
        public bool HasTextAnswer { get; private set; }
        public bool HasBooleanAnswer { get; private set; }
    }

    public class Answer
    {
        public Answer(Models.Answer answer)
        {
            AnswerId = answer.AnswerId;
            AnswerText = answer.AnswerText;
            AnswerBoolean = answer.AnswerBoolean;
            LastModifiedDate = answer.LastModifiedDate;
            LastModifiedByUserId = answer.LastModifiedByUserId;
        }

        public int AnswerId { get; private set; }
        public string AnswerText { get; private set; }
        public bool? AnswerBoolean { get; private set; }
        public DateTime LastModifiedDate { get; private set; }
        public Guid LastModifiedByUserId { get; private set; }
    }
}