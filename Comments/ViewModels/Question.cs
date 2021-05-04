using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Comments.ViewModels
{
	public class Question : Location
    {
        public Question() { } //only here for model binding. don't use it in code.
        public Question(Models.Location location, Models.Question question) : base(location.LocationId, location.SourceURI, location.HtmlElementID,
            location.RangeStart, location.RangeStartOffset, location.RangeEnd, location.RangeEndOffset, location.Quote, location.Order, show: true, sectionHeader: location.SectionHeader, sectionNumber: location.SectionNumber)
        {
            QuestionId = question.QuestionId;
            QuestionText = question.QuestionText;
            QuestionTypeId = question.QuestionTypeId;
            LastModifiedByUserId = question.LastModifiedByUserId;
            LastModifiedDate = question.LastModifiedDate;
			QuestionType = new QuestionType(question.QuestionType);

			if (!(question.Answer is null))
                Answers = question.Answer.Select(answer => new Answer(answer)).ToList();
        }

        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int QuestionTypeId { get; set; }
        public string LastModifiedByUserId { get; set; }
        public DateTime LastModifiedDate { get; set; }
		public QuestionType QuestionType { get; set; }
		public IList<ViewModels.Answer> Answers { get; set; }
    }

	public enum QuestionTypes
	{
		Text,
		YesNo
	}

	public class QuestionType
	{
		public QuestionType() { } //only here for model binding. don't use it in code.
		public QuestionType(Models.QuestionType questionType)
		{
			QuestionTypeId = questionType.QuestionTypeId;
			Description = questionType.Description;
			HasTextAnswer = questionType.HasTextAnswer;
			HasBooleanAnswer = questionType.HasBooleanAnswer;
		}
		public int QuestionTypeId { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public QuestionTypes Type
		{
			get
			{
				if (HasBooleanAnswer && HasTextAnswer)
				{
					return QuestionTypes.YesNo;
				}
				return QuestionTypes.Text;
			}
			set { }
		}

		public string Description { get; set; }
		public bool HasTextAnswer { get; set; }
		public bool HasBooleanAnswer { get; set; }
	}
}
