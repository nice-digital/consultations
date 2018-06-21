using System;
using System.Collections.Generic;
using System.Linq;
using Comments.Common;
using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Comments.Test.UnitTests
{
    public class SubmitServiceTests : Infrastructure.TestBase
	{
		[Fact]
		public void Update_Comment_When_Submitted()
		{
			//Arrange
			ResetDatabase();
			var userId = Guid.NewGuid();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var consultationId = 1;

			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var authenticateService = new FakeAuthenticateService(authenticated: true);
			var consultationContext = new ConsultationsContext(_options, userService);

			var commentService = new CommentService(consultationContext, userService, authenticateService);
			var submitService = new SubmitService(consultationContext, userService, authenticateService);

			var locationId = AddLocation(sourceURI, _context);
			var commentId = AddComment(locationId, "Comment text", false, userId, StatusName.Draft, _context);

			//Act
			var commentsAndAnswers = commentService.GetCommentsAndAnswers(sourceURI, true);
			var result = submitService.SubmitCommentsAndAnswers(commentsAndAnswers);
			var comment = commentService.GetComment(commentId);

			var commentsSubmissionData = _context.SubmissionComment.Where(s => s.CommentId == commentId)
				.Include(s => s.Submission).First();

			//Assert
			result.rowsUpdated.ShouldBe(3);
			comment.comment.StatusId.ShouldBe(StatusName.Submitted);
			//commentsSubmissionData.SubmissionId.ShouldBe(1);
			//commentsSubmissionData.SubmissionCommentId.ShouldBe(1);
			commentsSubmissionData.CommentId.ShouldBe(commentId);
			commentsSubmissionData.Submission.SubmissionByUserId.ShouldBe(userId);
		}

		[Fact]
		public void Update_Answer_When_Submitted()
		{
			//Arrange
			ResetDatabase();
			var userId = Guid.NewGuid();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var consultationId = 1;

			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var authenticateService = new FakeAuthenticateService(authenticated: true);
			var consultationContext = new ConsultationsContext(_options, userService);

			var commentService = new CommentService(consultationContext, userService, authenticateService);
			var answerService = new AnswerService(consultationContext, userService);
			var submitService = new SubmitService(consultationContext, userService, authenticateService);

			var locationId = AddLocation(sourceURI, _context);
			var questionTypeId = AddQuestionType("Question Type", false, true);
			var questionId = AddQuestion(locationId, questionTypeId, "Question Text");
			var answerId = AddAnswer(questionId, userId, "Answer Text");

			//Act
			var commentsAndAnswers = commentService.GetCommentsAndAnswers(sourceURI, true);
			var result = submitService.SubmitCommentsAndAnswers(commentsAndAnswers);
			var answer = answerService.GetAnswer(answerId);

			var answerSubmissionData = _context.SubmissionAnswer.Where(s => s.AnswerId == answerId)
				.Include(s => s.Submission).First();

			//Assert
			result.rowsUpdated.ShouldBe(3);
			answer.answer.StatusId.ShouldBe(StatusName.Submitted);
			//answerSubmissionData.SubmissionId.ShouldBe(1);
			//answerSubmissionData.SubmissionAnswerId.ShouldBe(1);
			answerSubmissionData.AnswerId.ShouldBe(answerId);
			answerSubmissionData.Submission.SubmissionByUserId.ShouldBe(userId);
		}

		[Fact]
		public void Get_Users_Submission()
		{
			//Arrange
			ResetDatabase();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var userId = Guid.NewGuid();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var consultationContext = new ConsultationsContext(_options, userService);

			AddSubmittedCommentsAndAnswers(sourceURI, "Comment Text", "Question Text", "Answer Text", userId, consultationContext);

			var sourceURIs = new List<string>
			{
				ConsultationsUri.ConvertToConsultationsUri("/1/0/Review", CommentOn.Consultation)
			};

			//Act
			var results = consultationContext.GetAllCommentsAndQuestionsForDocument(sourceURIs, true);

			//Assert
			results.First().Comment.First().SubmissionComment.Count.ShouldBe(1);
		}
	}
}
