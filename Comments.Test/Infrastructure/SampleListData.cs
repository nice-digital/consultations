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
			return new ReviewPageViewModel()
			{
				CommentsAndQuestions = new CommentsAndQuestions(
					new List<Comment>()
					{
						new Comment(0, "consultations://./consultation/1/document/1/chapter/introduction", null, null, null, null, null, null, null, 1, DateTime.MinValue, Guid.Empty, "some comment", 1, true, null)
					},
					new List<Question>()
					{
						new Question(
							new Models.Location("consultations://./consultation/1/document/1/chapter/introduction", null, null, null, null, null, "quote", null, "section 1", null, null),
							new Models.Question(1, "question text", 1, null, new Comments.Models.QuestionType("description", true, false, null), null))
					},
					true,
					"sign in url",
					new ConsultationState(DateTime.MinValue, DateTime.MaxValue, true, true, true, false, true, null, null)
					)
			};
	    }
    }
}
