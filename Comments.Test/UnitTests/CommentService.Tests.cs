using System;
using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Xunit;
using Shouldly;
using System.Linq;
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
            var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
            var commentText = Guid.NewGuid().ToString();
            var userId = Guid.Empty;


            var locationId = AddLocation(sourceURI);
            var commentId = AddComment(locationId, commentText, isDeleted: false, createdByUserId: userId);

            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            var authenticateService = new FakeAuthenticateService(authenticated: true);
            var commentService = new CommentService(new ConsultationsContext(_options, userService), userService, authenticateService);

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

            var userId = Guid.Empty;
            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            var authenticateService = new FakeAuthenticateService(authenticated: true);

            var commentService = new CommentService(new ConsultationsContext(_options, userService), userService, authenticateService);

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
            var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
            var commentText = Guid.NewGuid().ToString();
            var userId = Guid.Empty;

            var locationId = AddLocation(sourceURI);
            var commentId = AddComment(locationId, commentText, isDeleted: false, createdByUserId: userId);
            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            var authenticateService = new FakeAuthenticateService(authenticated: true);
            var commentService = new CommentService(new ConsultationsContext(_options, userService), userService, authenticateService);

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
            var userId = Guid.Empty;

            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            var authenticateService = new FakeAuthenticateService(authenticated: true);

            var locationId = AddLocation(sourceURI);
            var commentId = AddComment(locationId, commentText, isDeleted: false, createdByUserId: userId);
            
            var commentService = new CommentService(new ConsultationsContext(_options, userService), userService, authenticateService);

            //Act
            commentService.DeleteComment(commentId);
            var viewModel = commentService.GetComment(commentId);

            //Assert
            viewModel.comment.ShouldBeNull();
            viewModel.validate.NotFound.ShouldBeTrue();
        }

        [Fact]
        public void Comments_CanBeCreated()
        {
            //Arrange
            ResetDatabase();
            var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
            var locationId = AddLocation(sourceURI);
            var userId = Guid.Empty;
            var commentText = Guid.NewGuid().ToString();
            var location = new Location(sourceURI, null, null, null, null, null, null, null, null);
            var comment = new Comment(locationId, userId, commentText, userId, location);
            var viewModel = new ViewModels.Comment(location, comment);
            
            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            var authenticateService = new FakeAuthenticateService(authenticated: true);
            var commentService = new CommentService(new ConsultationsContext(_options, userService), userService, authenticateService);

            //Act
            var result = commentService.CreateComment(viewModel);

            //Assert
            result.comment.CommentText.ShouldBe(commentText);
        }

        [Fact]
        public void No_Comments_returned_when_not_logged_in()
        {
            // Arrange
            ResetDatabase();
            var authenticateService = new FakeAuthenticateService(authenticated: false);
            var commentService = new CommentService(new ConsultationsContext(_options, _fakeUserService), FakeUserService.Get(isAuthenticated: false), authenticateService);

            // Act
            var viewModel = commentService.GetCommentsAndQuestions("consultations://./consultation/1/document/1/chapter/introduction");

            //Assert
            viewModel.IsAuthorised.ShouldBeFalse();
            viewModel.Comments.Count().ShouldBe(0);
            viewModel.Questions.Count().ShouldBe(0);
        }

        [Fact]
        public void Only_own_Comments_returned_when_logged_in()
        {
            // Arrange
            ResetDatabase();
            var userId = Guid.NewGuid();
            var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            var authenticateService = new FakeAuthenticateService(authenticated: true);

            var commentService = new CommentService(new ConsultationsContext(_options, userService), userService, authenticateService);
            var locationId = AddLocation(sourceURI);

            var expectedCommentId = AddComment(locationId, "current user's comment", isDeleted: false, createdByUserId: userId);
            AddComment(locationId, "another user's comment", isDeleted: false, createdByUserId: Guid.NewGuid());
            AddComment(locationId, "current user's deleted comment", isDeleted: true, createdByUserId: userId);

            // Act
            var viewModel = commentService.GetCommentsAndQuestions("consultations://./consultation/1/document/1/chapter/introduction");

            //Assert
            viewModel.Comments.Single().CommentId.ShouldBe(expectedCommentId);
        }
    }
}
