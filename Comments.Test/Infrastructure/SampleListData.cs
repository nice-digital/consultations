using Comments.ViewModels;
using System;
using System.Collections.Generic;
using Comments.Models;
using Comment = Comments.ViewModels.Comment;
using Question = Comments.ViewModels.Question;

namespace Comments.Test.Infrastructure
{
	public static class SampleListData
    {
		public static ReviewPageViewModel GetReviewPageViewModelWith1CommentAnd1Question()
		{
			var comment = new Comment(locationId: 0, sourceUri: "consultations://./consultation/1/document/1/chapter/introduction", htmlElementId: null, rangeStart: null, rangeStartOffset: null, rangeEnd: null, rangeEndOffset: null, quote: null, order: null, commentId: 1, lastModifiedDate: DateTime.MinValue, lastModifiedByUserId: Guid.Empty.ToString(), commentText: "some comment", statusId: 1, show: true, sectionHeader: null, sectionNumber: null);
			comment.CommenterEmail = "test@nice.org.uk";

			var answer = new ViewModels.Answer(1, "answerText", false, DateTime.MinValue, Guid.Empty.ToString(), 1, 1);
			answer.CommenterEmail = "test@nice.org.uk";

			var question = new Question(
						new Models.Location("consultations://./consultation/1/document/1/chapter/introduction", null, null, null, null, null, "quote", null, "section 1", "1", null, null),
						new Models.Question(1, "question text", 1, null, new Models.QuestionType("description", true, false, null), null));
			question.Answers = new List<ViewModels.Answer>() { answer };

			return new ReviewPageViewModel()
			{
				CommentsAndQuestions = new CommentsAndQuestions(
					new List<Comment>() { comment },
					new List<Question>() { question },
					true,
					"sign in url",
					new ConsultationState(DateTime.MinValue, DateTime.MaxValue, true, true, true, null, null, false)
					)
			};
		}
	}
}
