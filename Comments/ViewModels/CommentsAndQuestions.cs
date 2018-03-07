using System;
using System.Collections.Generic;
using System.Linq;

namespace Comments.ViewModels
{
    public class Location
    {
        public Location() { } //only here for model binding. don't use it in code.
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

        public int LocationId { get; set; }
        public int ConsultationId { get; set; }
        public int? DocumentId { get; set; }
        public string ChapterSlug { get; set; }
        public string SectionSlug { get; set; }
        public string RangeStart { get; set; }
        public int? RangeStartOffset { get; set; }
        public string RangeEnd { get; set; }
        public int? RangeEndOffset { get; set; }
        public string Quote { get; set; }
    }

    public class Comment : Location
    {
        public Comment() { } //only here for model binding. don't use it in code.

        public Comment(int locationId, int consultationId, int? documentId, string chapterSlug, string sectionSlug, string rangeStart, int? rangeStartOffset, string rangeEnd, int? rangeEndOffset, string quote, int commentId, DateTime lastModifiedDate, Guid lastModifiedByUserId, string commentText) : base(locationId, consultationId, documentId, chapterSlug, sectionSlug, rangeStart, rangeStartOffset, rangeEnd, rangeEndOffset, quote)
        {
            CommentId = commentId;
            LastModifiedDate = lastModifiedDate;
            LastModifiedByUserId = lastModifiedByUserId;
            CommentText = commentText;
        }

        public Comment(Models.Location location, Models.Comment comment) : base(location.LocationId, location.ConsultationId, location.DocumentId, location.ChapterSlug, location.SectionSlug, 
            location.RangeStart, location.RangeStartOffset, location.RangeEnd, location.RangeEndOffset, location.Quote)
        {
            CommentId = comment.CommentId;
            LastModifiedDate = comment.LastModifiedDate;
            LastModifiedByUserId = comment.LastModifiedByUserId;
            CommentText = comment.CommentText;
        }

        public int CommentId { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public Guid LastModifiedByUserId { get; set; }
        public string CommentText { get; set; }
    }

    public class Question : Location
    {
        public Question() { } //only here for model binding. don't use it in code.
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