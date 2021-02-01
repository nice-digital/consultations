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
using Comment = Comments.Models.Comment;

namespace Comments.Test.UnitTests
{
    public class SubmitServiceTests : Infrastructure.TestBase
	{
		[Fact]
		public void Update_Comment_When_Submitted()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();

			var userId = Guid.NewGuid().ToString();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";

			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var consultationContext = new ConsultationsContext(_options, userService, _fakeEncryption);
			var submitService = new SubmitService(consultationContext, userService, _consultationService);
			var commentService = new CommentService(consultationContext, userService, _consultationService, _fakeHttpContextAccessor);

			var locationId = AddLocation(sourceURI, _context);
			var commentId = AddComment(locationId, "Comment text", userId, (int)StatusName.Draft, _context);

			//Act
			var commentsAndQuestions = commentService.GetCommentsAndQuestions(sourceURI, _urlHelper);
			var result = submitService.Submit(new ViewModels.Submission(commentsAndQuestions.Comments, new List<ViewModels.Answer>()));

			var comment = commentService.GetComment(commentId);

			var commentsSubmissionData = _context.SubmissionComment.Where(s => s.CommentId == commentId)
				.Include(s => s.Submission).First();

			//Assert
			result.rowsUpdated.ShouldBe(3);
			comment.comment.StatusId.ShouldBe((int)StatusName.Submitted);
			comment.comment.Status.StatusId.ShouldBe((int)StatusName.Submitted);
			commentsSubmissionData.CommentId.ShouldBe(commentId);
			commentsSubmissionData.Submission.SubmissionByUserId.ShouldBe(userId);
		}

		[Fact]
		public void Update_Answer_When_Submitted()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();

			var userId = Guid.NewGuid().ToString();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";

			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var consultationContext = new ConsultationsContext(_options, userService, _fakeEncryption);
			var submitService = new SubmitService(consultationContext, userService, _consultationService);
			var commentService = new CommentService(consultationContext, userService, _consultationService, _fakeHttpContextAccessor);
			var answerService = new AnswerService(consultationContext, userService);

			var locationId = AddLocation(sourceURI, _context);
			var questionTypeId = 99;
			var questionId = AddQuestion(locationId, questionTypeId, "Question Label");
			var answerId = AddAnswer(questionId, userId, "Answer Label");

			//Act
			var commentsAndQuestions = commentService.GetCommentsAndQuestions(sourceURI, _urlHelper);
			var result = submitService.Submit(new ViewModels.Submission(new List<ViewModels.Comment>(), commentsAndQuestions.Questions.First().Answers));

			var answer = answerService.GetAnswer(answerId);

			var answerSubmissionData = _context.SubmissionAnswer.Where(s => s.AnswerId == answerId)
				.Include(s => s.Submission).First();

			//Assert
			result.rowsUpdated.ShouldBe(3);
			answer.answer.StatusId.ShouldBe((int)StatusName.Submitted);
			answer.answer.Status.StatusId.ShouldBe((int)StatusName.Submitted);
			answerSubmissionData.AnswerId.ShouldBe(answerId);
			answerSubmissionData.Submission.SubmissionByUserId.ShouldBe(userId);
		}

		[Fact]
		public void Get_Users_Submission()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var userId = Guid.NewGuid().ToString();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var consultationContext = new ConsultationsContext(_options, userService, _fakeEncryption);

			AddSubmittedCommentsAndAnswers(sourceURI, "Comment Label", "Question Label", "Answer Label", userId, consultationContext);

			var sourceURIs = new List<string>
			{
				ConsultationsUri.ConvertToConsultationsUri("/1/0/Review", CommentOn.Consultation)
			};

			//Act
			var results = consultationContext.GetAllCommentsAndQuestionsForDocument(sourceURIs, true);

			//Assert
			results.First().Comment.First().SubmissionComment.Count.ShouldBe(1);
			results.First().Comment.First().Status.ShouldNotBeNull();
		}

		[Theory]
		[InlineData(null, false)]
		[InlineData("", false)]
		[InlineData("  ", false)]
		[InlineData("consultations://./consultation/10/document/1/chapter/introduction", true)]
		[InlineData("consultations://./consultation/10/document/2/chapter/introduction", true)]
		[InlineData("consultations://./consultation/10/document/1/chapter/anotherchaptertitle", true)]
		[InlineData("consultations://./consultation/10/document/1", true)]
		[InlineData("consultations://./consultation/10", true)]
		[InlineData("consultations://./consultation/1", false)]
		[InlineData("consultations://./consultation/2", false)]
		public void Has_Submitted_Comments_Or_Answers_For_Chapter_SourceURI(string consultationSourceURI, bool expectedResult)
		{
			//Arrange
			var userId = Guid.NewGuid().ToString();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var consultationContext = new ConsultationsContext(_options, userService, _fakeEncryption);
			var consultationService = new ConsultationService(consultationContext, null, null, userService);
			//var submitService = new SubmitService(consultationContext, userService, _consultationService);
			AddSubmittedCommentsAndAnswers("consultations://./consultation/10/document/1/chapter/introduction", "Comment Label", "Question Label", "Answer Label", userId, consultationContext);

			//Act
			var actualResult = consultationService.GetSubmittedDate(consultationSourceURI).HasValue;

			//Assert
			actualResult.ShouldBe(expectedResult);
		}

		[Theory]
		[InlineData(null, false)]
		[InlineData("", false)]
		[InlineData("  ", false)]
		[InlineData("consultations://./consultation/10/document/10/chapter/introduction", true)]
		[InlineData("consultations://./consultation/10/document/2/chapter/introduction", true)]
		[InlineData("consultations://./consultation/10/document/10/chapter/anotherchaptertitle", true)]
		[InlineData("consultations://./consultation/10/document/10", true)]
		[InlineData("consultations://./consultation/10/document/1", true)]
		[InlineData("consultations://./consultation/10", true)]
		[InlineData("consultations://./consultation/1", false)]
		[InlineData("consultations://./consultation/2", false)]
		public void Has_Submitted_Comments_Or_Answers_For_Document_SourceURI(string consultationSourceURI, bool expectedResult)
		{
			//Arrange
			var userId = Guid.NewGuid().ToString();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var consultationContext = new ConsultationsContext(_options, userService, _fakeEncryption);
			//var submitService = new SubmitService(consultationContext, userService, _consultationService);
			var consultationService = new Services.ConsultationService(consultationContext, null, null, userService);
			AddSubmittedCommentsAndAnswers("consultations://./consultation/10/document/10", "Comment Label", "Question Label", "Answer Label", userId, consultationContext);

			//Act
			var actualResult = consultationService.GetSubmittedDate(consultationSourceURI).HasValue;

			//Assert
			actualResult.ShouldBe(expectedResult);
		}

		[Theory]
		[InlineData(null, false)]
		[InlineData("", false)]
		[InlineData("  ", false)]
		[InlineData("consultations://./consultation/10/document/1/chapter/introduction", true)]
		[InlineData("consultations://./consultation/10/document/2/chapter/introduction", true)]
		[InlineData("consultations://./consultation/10/document/1/chapter/anotherchaptertitle", true)]
		[InlineData("consultations://./consultation/10/document/1", true)]
		[InlineData("consultations://./consultation/10", true)]
		[InlineData("consultations://./consultation/1", false)]
		[InlineData("consultations://./consultation/2", false)]
		public void Has_Submitted_Comments_Or_Answers_For_Consultation_SourceURI(string consultationSourceURI, bool expectedResult)
		{
			//Arrange
			var userId = Guid.NewGuid().ToString();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var consultationContext = new ConsultationsContext(_options, userService, _fakeEncryption);
			var consultationService = new ConsultationService(consultationContext, null, null, userService);
			//var submitService = new SubmitService(consultationContext, userService, _consultationService);
			AddSubmittedCommentsAndAnswers("consultations://./consultation/10", "Comment Label", "Question Label", "Answer Label", userId, consultationContext);

			//Act
			var actualResult = consultationService.GetSubmittedDate(consultationSourceURI).HasValue;

			//Assert
			actualResult.ShouldBe(expectedResult);
		}

		[Theory]
		[InlineData(null, false)]
		[InlineData("", false)]
		[InlineData("  ", false)]
		[InlineData("consultations://./consultation/10/document/1/chapter/introduction", true)]
		[InlineData("consultations://./consultation/10/document/2/chapter/introduction", true)]
		[InlineData("consultations://./consultation/10/document/1/chapter/anotherchaptertitle", true)]
		[InlineData("consultations://./consultation/10/document/1", true)]
		[InlineData("consultations://./consultation/10", true)]
		[InlineData("consultations://./consultation/1", false)]
		[InlineData("consultations://./consultation/2", false)]
		public void Has_Submitted_Comments_Or_Answers_For_Consultation_SourceURI_With_Only_Comments(string consultationSourceURI, bool expectedResult)
		{
			//Arrange
			var userId = Guid.NewGuid().ToString();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var consultationContext = new ConsultationsContext(_options, userService, _fakeEncryption);
			var consultationService = new ConsultationService(consultationContext, null, null, userService);
			AddSubmittedComments("consultations://./consultation/10", "Comment Label", "Question Label", "Answer Label", userId, consultationContext);

			//Act
			var actualResult = consultationService.GetSubmittedDate(consultationSourceURI).HasValue;

			//Assert
			actualResult.ShouldBe(expectedResult);
		}

		[Theory]
		[InlineData(null, false)]
		[InlineData("", false)]
		[InlineData("  ", false)]
		[InlineData("consultations://./consultation/10/document/1/chapter/introduction", true)]
		[InlineData("consultations://./consultation/10/document/2/chapter/introduction", true)]
		[InlineData("consultations://./consultation/10/document/1/chapter/anotherchaptertitle", true)]
		[InlineData("consultations://./consultation/10/document/1", true)]
		[InlineData("consultations://./consultation/10", true)]
		[InlineData("consultations://./consultation/1", false)]
		[InlineData("consultations://./consultation/2", false)]
		public void Has_Submitted_Comments_Or_Answers_For_Consultation_SourceURI_With_Only_Answers(string consultationSourceURI, bool expectedResult)
		{
			//Arrange
			var userId = Guid.NewGuid().ToString();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var consultationContext = new ConsultationsContext(_options, userService, _fakeEncryption);
			var consultationService = new ConsultationService(consultationContext, null, null, userService);
			AddSubmittedQuestionsWithAnswers("consultations://./consultation/10", "Comment Label", "Question Label", "Answer Label", userId, consultationContext);

			//Act
			var actualResult = consultationService.GetSubmittedDate(consultationSourceURI).HasValue;

			//Assert
			actualResult.ShouldBe(expectedResult);
		}

		[Fact]
		public void Duplicate_Comment()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();

			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var consultationId = 1;
			var organisationId = 1;
			var authorisationSession = Guid.NewGuid();

			var organisationAuthorisationId = TestBaseDBHelpers.AddOrganisationAuthorisationWithLocation(organisationId, consultationId, _context);
			var organisationUserId = TestBaseDBHelpers.AddOrganisationUser(_context, organisationAuthorisationId, authorisationSession, null);

			var userService = FakeUserService.Get(true, "Benjamin Button", null, TestUserType.NotAuthenticated, false, organisationUserId);
			var consultationContext = new ConsultationsContext(_options, userService, _fakeEncryption);
			var submitService = new SubmitService(consultationContext, userService, _consultationService);
			var commentService = new CommentService(consultationContext, userService, _consultationService, _fakeHttpContextAccessor);

			var locationId = AddLocation(sourceURI, _context);
			AddComment(locationId, "Comment text", null, (int)StatusName.Draft, _context, organisationUserId);

			//Act
			var commentsAndQuestions = commentService.GetCommentsAndQuestions(sourceURI, _urlHelper);
			var result = submitService.SubmitToLead(new ViewModels.Submission(commentsAndQuestions.Comments, new List<ViewModels.Answer>()), "testemail@nice.org.uk", authorisationSession);

			//Assert
			result.rowsUpdated.ShouldBe(2);
			result.context.Comment.First().StatusId.ShouldBe(2);
			result.context.Comment.First().OrganisationId.ShouldBe(null);
			result.context.Comment.First().ParentCommentId.ShouldBe(null);
			result.context.Comment.First().ChildComments.First().StatusId.ShouldBe(1);
			result.context.Comment.First().ChildComments.First().OrganisationId.ShouldBe(1);
			result.context.Comment.First().ChildComments.First().ParentCommentId.ShouldBe(1);
			result.context.Comment.First().ChildComments.First().CommentText.ShouldBe("Comment text");
		}

		[Fact]
		public void Duplicate_Answer()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();

			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var questionTypeId = 99;
			var consultationId = 1;
			var organisationId = 1;
			var authorisationSession = Guid.NewGuid();

			var organisationAuthorisationId = TestBaseDBHelpers.AddOrganisationAuthorisationWithLocation(organisationId, consultationId, _context);
			var organisationUserId = TestBaseDBHelpers.AddOrganisationUser(_context, organisationAuthorisationId, authorisationSession, null);

			var userService = FakeUserService.Get(true, "Benjamin Button", null, TestUserType.NotAuthenticated, false, organisationUserId);
			var consultationContext = new ConsultationsContext(_options, userService, _fakeEncryption);
			var submitService = new SubmitService(consultationContext, userService, _consultationService);
			var commentService = new CommentService(consultationContext, userService, _consultationService, _fakeHttpContextAccessor);

			var locationId = AddLocation(sourceURI, _context);
			var questionId = AddQuestion(locationId, questionTypeId, "Question Label");
			AddAnswer(questionId, null, "Answer Label", (int)StatusName.Draft, _context, organisationUserId);

			//Act
			var commentsAndQuestions = commentService.GetCommentsAndQuestions(sourceURI, _urlHelper);
			var result = submitService.SubmitToLead(new ViewModels.Submission(new List<ViewModels.Comment>(), commentsAndQuestions.Questions.First().Answers), "testemail@nice.org.uk", authorisationSession);

			//Assert
			result.rowsUpdated.ShouldBe(2);
			result.context.Answer.First().StatusId.ShouldBe(2);
			result.context.Answer.First().ChildAnswers.First().StatusId.ShouldBe(1);
		}
	}
}
