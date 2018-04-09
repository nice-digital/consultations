using System;
using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using NICE.Feeds;
using NICE.Feeds.Tests.Infrastructure;
using Xunit;
using Shouldly;
using System.Linq;

namespace Comments.Test.UnitTests
{
    public class CommentServiceTests : Infrastructure.TestBase
    {
        [Fact]
        public void Comments_Get()
        {
            // Arrange
            ResetDatabase();
            var sourceURI = "/consultations/1/1/introduction";
            var commentText = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid();

            var locationId = AddLocation(sourceURI);
            var commentId = AddComment(locationId, commentText, isDeleted: false, createdByUserId: userId);
            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            var commentService = new CommentService(new ConsultationsContext(_options, userService), userService);

            // Act
            var viewModel = commentService.GetComment(commentId);

            //Assert
            viewModel.comment.CommentText.ShouldBe(commentText);
            //viewModel.CommentText.ShouldBe(commentText);
        }

        [Fact]
        public void Comments_CanBeEdited()
        {
            //Arrange
            ResetDatabase();
            var sourceURI = "/consultations/1/1/introduction";
            var commentText = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid();

            var locationId = AddLocation(sourceURI);
            var commentId = AddComment(locationId, commentText, isDeleted: false, createdByUserId: userId);
            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            var commentService = new CommentService(new ConsultationsContext(_options, userService), userService);

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
            var sourceURI = "/consultations/1/1/introduction";
            var commentText = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid();

            var locationId = AddLocation(sourceURI);
            var commentId = AddComment(locationId, commentText, isDeleted: false, createdByUserId: userId);
            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            var commentService = new CommentService(new ConsultationsContext(_options, userService), userService);

            //Act
            commentService.DeleteComment(commentId);
            var viewModel = commentService.GetComment(commentId);

            //Assert
            viewModel.ShouldBeNull();
        }

        [Fact]
        public void Comments_CanBeCreated()
        {
            //Arrange
            ResetDatabase();
            var sourceURI = "/consultations/1/1/introduction";
            var locationId = AddLocation(sourceURI);
            var userId = Guid.NewGuid();
            var commentText = Guid.NewGuid().ToString();
            var location = new Location(sourceURI, null, null, null, null, null, null, null, null);
            var comment = new Comment(locationId, userId, commentText, userId, location);
            var viewModel = new ViewModels.Comment(location, comment);
            
            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            var commentService = new CommentService(new ConsultationsContext(_options, userService), userService);

            //Act
            var result = commentService.CreateComment(viewModel);

            //Assert
            result.comment.CommentId.ShouldBe(1);
            result.comment.CommentText.ShouldBe(commentText);
        }

        [Fact]
        public void No_Comments_returned_when_not_logged_in()
        {
            // Arrange
            ResetDatabase();
            var commentService = new CommentService(new ConsultationsContext(_options, _fakeUserService), FakeUserService.Get(isAuthenticated: false));

            // Act
            var viewModel = commentService.GetCommentsAndQuestions("/consultations/1/1/introduction");

            //Assert
            viewModel.IsAuthenticated.ShouldBeFalse();
            viewModel.Comments.Count().ShouldBe(0);
            viewModel.Questions.Count().ShouldBe(0);
        }

        [Fact]
        public void Only_own_Comments_returned_when_logged_in()
        {
            // Arrange
            ResetDatabase();
            var userId = Guid.NewGuid();
            var sourceURI = "/consultations/1/1/introduction";
            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            var commentService = new CommentService(new ConsultationsContext(_options, userService), userService);
            var locationId = AddLocation(sourceURI);

            var expectedCommentId = AddComment(locationId, "current user's comment", isDeleted: false, createdByUserId: userId);
            var anotherPersonsCommentId = AddComment(locationId, "another user's comment", isDeleted: false, createdByUserId: Guid.NewGuid());
            var ownDeletedCommentId = AddComment(locationId, "current user's deleted comment", isDeleted: true, createdByUserId: userId);

            // Act
            var viewModel = commentService.GetCommentsAndQuestions("/consultations/1/1/introduction");

            //Assert
            viewModel.Comments.Single().CommentId.ShouldBe(expectedCommentId);
        }
    }
}
