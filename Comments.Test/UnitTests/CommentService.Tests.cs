using System;
using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Xunit;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Comments.ViewModels;
using Microsoft.EntityFrameworkCore;
using Comment = Comments.Models.Comment;
using Location = Comments.Models.Location;

namespace Comments.Test.UnitTests
{
    public class CommentServiceTests : Infrastructure.TestBase
    {
        [Fact]
        public void Comments_Get()
        {
            // Arrange
            ResetDatabase();
	        _context.Database.EnsureCreated();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
            var commentText = Guid.NewGuid().ToString();
            var userId = Guid.Empty.ToString();


            var locationId = AddLocation(sourceURI);
            var commentId = AddComment(locationId, commentText, createdByUserId: userId);

            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
	        var context = new ConsultationsContext(_options, userService, _fakeEncryption);
	        //var submitService = new SubmitService(context, userService, _consultationService);
			var commentService = new CommentService(context, userService, _consultationService, _fakeHttpContextAccessor);

            // Act
            var viewModel = commentService.GetComment(commentId);

            //Assert
            viewModel.comment.CommentText.ShouldBe(commentText);
        }

        [Fact]
        public void Comments_Get_Record_Not_Found()
        {
            // Arrange
            ResetDatabase();

            var userId = Guid.Empty.ToString();
            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);

	        var context = new ConsultationsContext(_options, userService, _fakeEncryption);
			var commentService = new CommentService(context, userService, _consultationService, _fakeHttpContextAccessor);

            // Act
            var viewModel = commentService.GetComment(1);

            //Assert
            viewModel.validate.NotFound.ShouldBeTrue();
            viewModel.comment.ShouldBeNull();
        }

        [Fact]
        public void Comments_CanBeEdited()
        {
            //Arrange
            ResetDatabase();
	        _context.Database.EnsureCreated();

			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
            var commentText = Guid.NewGuid().ToString();
            var userId = Guid.Empty.ToString();

            var locationId = AddLocation(sourceURI);
            var commentId = AddComment(locationId, commentText, createdByUserId: userId);

            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
	        var context = new ConsultationsContext(_options, userService, _fakeEncryption);
			var commentService = new CommentService(context, userService, _consultationService, _fakeHttpContextAccessor);

            var viewModel = commentService.GetComment(commentId);
            var updatedCommentText = Guid.NewGuid().ToString();

            viewModel.comment.CommentText = updatedCommentText;

            //Act
            commentService.EditComment(commentId, viewModel.comment);
            viewModel = commentService.GetComment(commentId);

            //Assert
            viewModel.comment.CommentText.ShouldBe(updatedCommentText);
        }

        [Fact]
        public void Comments_CanBeDeleted()
        {
            //Arrange
            ResetDatabase();
            var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
            var commentText = Guid.NewGuid().ToString();
            var userId = Guid.Empty.ToString();

            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);

            var locationId = AddLocation(sourceURI);
            var commentId = AddComment(locationId, commentText, createdByUserId: userId);

	        var context = new ConsultationsContext(_options, userService, _fakeEncryption);
	        //var submitService = new SubmitService(context, userService, _consultationService);
			var commentService = new CommentService(context, userService, _consultationService, _fakeHttpContextAccessor);

            //Act
            commentService.DeleteComment(commentId);
            var viewModel = commentService.GetComment(commentId);

            //Assert
            viewModel.comment.ShouldBeNull();
            viewModel.validate.NotFound.ShouldBeTrue();
        }

        [Fact]
        public async Task Comments_CanBeCreated()
        {
            //Arrange
            ResetDatabase();
	        _context.Database.EnsureCreated();

            var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
            var locationId = AddLocation(sourceURI);
            var userId = Guid.Empty.ToString();
            var commentText = Guid.NewGuid().ToString();
            var location = new Location(sourceURI, null, null, null, null, null, null, null, null, null, null, null);
            var comment = new Comment(locationId, userId, commentText, userId, location, 1, null);
            var viewModel = new ViewModels.Comment(location, comment);
            
            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
	        var context = new ConsultationsContext(_options, userService, _fakeEncryption);
			//var submitService = new SubmitService(context, userService, _consultationService);
			var commentService = new CommentService(context, userService, _consultationService, _fakeHttpContextAccessor);

            //Act
            var result = await commentService.CreateComment(viewModel);

            //Assert
            result.comment.CommentText.ShouldBe(commentText);
        }

        [Fact]
        public async Task No_Comments_returned_when_not_logged_in()
        {
            // Arrange
            ResetDatabase();
	        var context = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption);
	        //var submitService = new SubmitService(context, _fakeUserService, _consultationService);
			var commentService = new CommentService(new ConsultationsContext(_options, _fakeUserService, _fakeEncryption), FakeUserService.Get(isAuthenticated: false), _consultationService, _fakeHttpContextAccessor);

            // Act
            var viewModel = await commentService.GetCommentsAndQuestions("consultations://./consultation/1/document/1/chapter/introduction", _urlHelper);

            //Assert
            viewModel.IsAuthorised.ShouldBeFalse();
            viewModel.Comments.Count().ShouldBe(0);
            viewModel.Questions.Count().ShouldBe(0);
        }

        [Fact]
        public async Task Only_own_Comments_returned_when_logged_in()
        {
            // Arrange
            ResetDatabase();
	        _context.Database.EnsureCreated();

			var userId = Guid.NewGuid().ToString();
            var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
	        var context = new ConsultationsContext(_options, userService, _fakeEncryption);
	        //var submitService = new SubmitService(context, userService, _consultationService);
			var commentService = new CommentService(new ConsultationsContext(_options, userService, _fakeEncryption), userService, _consultationService, _fakeHttpContextAccessor);
            var locationId = AddLocation(sourceURI);

            var expectedCommentId = AddComment(locationId, "current user's comment", createdByUserId: userId);
            AddComment(locationId, "another user's comment", createdByUserId: Guid.NewGuid().ToString());

            // Act
            var viewModel = await commentService.GetCommentsAndQuestions("consultations://./consultation/1/document/1/chapter/introduction", _urlHelper);

            //Assert
            viewModel.Comments.Single().CommentId.ShouldBe(expectedCommentId);
        }

        [Fact]
        public async Task CommentsQuestionsAndAnswers_OnlyReturnOwnComments()
        {
            // Arrange
            ResetDatabase();
	        _context.Database.EnsureCreated();

            var URI = "consultations://./consultation/1/document/1/chapter/intro";

            var someText = Guid.NewGuid().ToString();
            var createdByUserId = Guid.Empty.ToString();
            var anotherUserId = Guid.NewGuid().ToString();

            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: createdByUserId);

            AddCommentsAndQuestionsAndAnswers(URI, "My Comment", someText, someText, createdByUserId, (int)StatusName.Draft, _context);
            AddCommentsAndQuestionsAndAnswers(URI, "Another Users Comment", someText, someText, anotherUserId, (int)StatusName.Draft, _context);
	        var context = new ConsultationsContext(_options, userService, _fakeEncryption);
	        //var submitService = new SubmitService(context, _fakeUserService, _consultationService);
			var commentService = new CommentService(context, _fakeUserService, _consultationService, _fakeHttpContextAccessor);

            // Act    
            var viewModel = await commentService.GetCommentsAndQuestions(URI, _urlHelper);

			//Assert
			//commentService.GetComment(1).comment.CommentText.ShouldBe("My Comment");
			//commentService.GetComment(2).validate.NotFound.ShouldBeTrue();
            viewModel.Comments.Count().ShouldBe(1);
        }

        [Fact]
        public async Task CommentsQuestionsAndAnswers_ReturnAllOwnCommentsForReviewingConsultation()
        {
            // Arrange
            ResetDatabase();
	        _context.Database.EnsureCreated();

			var ChapterIntroURI = "consultations://./consultation/1/document/1/chapter/introduction";
            var ChaperOverviewURI = "consultations://./consultation/1/document/1/chapter/overview";

            var DocumentOneURI = "consultations://./consultation/1/document/1";
            var DocumentTwoURI = "consultations://./consultation/1/document/2/chapter/guidelines";

            var ConsultationOneURI = "consultations://./consultation/1";
            var ConsultationTwoURI = "consultations://./consultation/2/document/1/chapter/ERROR";

            var ConsultationUserError = "consultations://./consultation/1/document/1/chapter/USERERROR";

            var commentText = Guid.NewGuid().ToString();
            var questionText = Guid.NewGuid().ToString();
            var answerText = Guid.NewGuid().ToString();
            var createdByUserId = Guid.Empty.ToString();
            var anotherUserId = Guid.NewGuid().ToString();

            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: createdByUserId);

            AddCommentsAndQuestionsAndAnswers(ChapterIntroURI, commentText, questionText, answerText, createdByUserId, (int)StatusName.Draft, _context);
            AddCommentsAndQuestionsAndAnswers(ChaperOverviewURI, commentText, questionText, answerText, createdByUserId, (int)StatusName.Draft, _context);
            AddCommentsAndQuestionsAndAnswers(DocumentOneURI, commentText, questionText, answerText, createdByUserId, (int)StatusName.Draft, _context);
            AddCommentsAndQuestionsAndAnswers(ConsultationOneURI, commentText, questionText, answerText, createdByUserId, (int)StatusName.Draft, _context);
            AddCommentsAndQuestionsAndAnswers(DocumentTwoURI, commentText, questionText, answerText, createdByUserId, (int)StatusName.Draft, _context);
            AddCommentsAndQuestionsAndAnswers(ConsultationTwoURI, commentText, questionText, answerText, createdByUserId, (int)StatusName.Draft, _context);
            AddCommentsAndQuestionsAndAnswers(ChaperOverviewURI, "Another Users Comment", questionText, answerText, anotherUserId, (int)StatusName.Draft, _context);

            var context = new ConsultationsContext(_options, userService, _fakeEncryption);
	        //var submitService = new SubmitService(context, _fakeUserService, _consultationService);
			var commentService = new CommentService(context, _fakeUserService, _consultationService, _fakeHttpContextAccessor);

            // Act    
            var viewModel = await commentService.GetCommentsAndQuestions("/1/review", _urlHelper);

            //Assert
            //commentService.GetComment(6).comment.ShouldNotBeNull();
            //commentService.GetComment(7).validate.NotFound.ShouldBeTrue();
            viewModel.Comments.Count().ShouldBe(5);
        }



        [Fact]
        public async Task Only_own_Comments_returned_for_comment_list_when_has_organisation_session_cookie_exists()
        {
	        // Arrange
	        ResetDatabase();
	        _context.Database.EnsureCreated();

	        var sessionId = Guid.NewGuid();
	        var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
	        const int organisationUserId = 1;

			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: null, organisationUserId: organisationUserId);
	        var context = new ConsultationsContext(_options, userService, _fakeEncryption);
	        var commentService = new CommentService(new ConsultationsContext(_options, userService, _fakeEncryption), userService, _consultationService, _fakeHttpContextAccessor);
	        var locationId = AddLocation(sourceURI);

	        var expectedCommentId = AddComment(locationId, "current user's comment", createdByUserId: null, organisationUserId: organisationUserId);
	        AddComment(locationId, "another user's comment logged in using auth", createdByUserId: Guid.NewGuid().ToString());
	        AddComment(locationId, "another user's comment logged in with cookie", createdByUserId: null, organisationUserId: 9999);

			// Act
			var viewModel = await commentService.GetCommentsAndQuestions("/1/1/introduction", _urlHelper);

	        //Assert
	        viewModel.Comments.Single().CommentId.ShouldBe(expectedCommentId);
        }

        [Fact] public void Only_return_comments_from_my_organisation_which_have_been_submitted_to_lead()
        {
	        // Arrange
	        ResetDatabase();
	        _context.Database.EnsureCreated();

	        var sessionId = Guid.NewGuid();
	        var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
	        const int organisationUserId = 1;
			const int organisationId = 1;

	        var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: null, organisationUserId: organisationUserId);
	        var context = new ConsultationsContext(_options, userService, _fakeEncryption);
	        var commentService = new CommentService(new ConsultationsContext(_options, userService, _fakeEncryption), userService, _consultationService, _fakeHttpContextAccessor);
	        var locationId = AddLocation(sourceURI);

	        AddComment(locationId, "current user's comment", createdByUserId: null, organisationUserId: organisationUserId, organisationId: organisationId);
	        AddComment(locationId, "another user from my organisations comment not submitted", null, status: (int)StatusName.Draft, organisationUserId: 9999, organisationId: organisationId);
			AddComment(locationId, "another user from my organisations comment submitted to lead", null, status: (int)StatusName.SubmittedToLead, organisationUserId: 8888, organisationId: organisationId);
			AddComment(locationId, "another user from a different organisation comment", createdByUserId: null, status: (int)StatusName.SubmittedToLead, organisationUserId: 7777, organisationId: 2);
			AddComment(locationId, "an individual users comment", createdByUserId: "1", status: (int)StatusName.Submitted);

			// Act
			var viewModel = commentService.GetCommentsAndQuestionsFromOtherOrganisationCommenters("/1/1/introduction", _urlHelper);

	        //Assert
	        viewModel.Comments.Count.Equals(1);
			viewModel.Comments.Single().CommentText.ShouldBe("another user from my organisations comment submitted to lead");
        }
		
	}
}


