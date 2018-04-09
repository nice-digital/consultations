using System;
using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using NICE.Feeds;
using NICE.Feeds.Tests.Infrastructure;
using Xunit;
using Shouldly;

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

            var locationId = AddLocation(sourceURI);
            var commentId = AddComment(locationId, commentText, isDeleted: false);
            var feedReaderService = new FeedReader(Feed.ConsultationCommentsListDetailMulitpleDoc);
            var commentService = new CommentService(new ConsultationsContext(_options), new ConsultationService(new FeedConverterService(feedReaderService), new FakeLogger<ConsultationService>()));

            // Act
            var viewModel = commentService.GetComment(commentId);

            //Assert
            viewModel.CommentText.ShouldBe(commentText);
        }

        [Fact]
        public void Comments_CanBeEdited()
        {
            //Arrange
            ResetDatabase();
            var sourceURI = "/consultations/1/1/introduction";
            var commentText = Guid.NewGuid().ToString();

            var locationId = AddLocation(sourceURI);
            var commentId = AddComment(locationId, commentText, isDeleted: false);
            var feedReaderService = new FeedReader(Feed.ConsultationCommentsListDetailMulitpleDoc);
            var commentService = new CommentService(new ConsultationsContext(_options), new ConsultationService(new FeedConverterService(feedReaderService), new FakeLogger<ConsultationService>()));

            var viewModel = commentService.GetComment(commentId);
            var updatedCommentText = Guid.NewGuid().ToString();

            viewModel.CommentText = updatedCommentText;

            //Act
            commentService.EditComment(commentId, viewModel);
            viewModel = commentService.GetComment(commentId);

            //Assert
            viewModel.CommentText.ShouldBe(updatedCommentText);
        }

        [Fact]
        public void Comments_CanBeDeleted()
        {
            //Arrange
            ResetDatabase();
            var sourceURI = "/consultations/1/1/introduction";
            var commentText = Guid.NewGuid().ToString();

            var locationId = AddLocation(sourceURI);
            var commentId = AddComment(locationId, commentText, isDeleted: false);
            var feedReaderService = new FeedReader(Feed.ConsultationCommentsListDetailMulitpleDoc);
            var commentService = new CommentService(new ConsultationsContext(_options), new ConsultationService(new FeedConverterService(feedReaderService), new FakeLogger<ConsultationService>()));

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


            var feedReaderService = new FeedReader(Feed.ConsultationCommentsListDetailMulitpleDoc);
            var commentService = new CommentService(new ConsultationsContext(_options), new ConsultationService(new FeedConverterService(feedReaderService), new FakeLogger<ConsultationService>()));

            //Act
            var result = commentService.CreateComment(viewModel);

            //Assert
            result.CommentId.ShouldBe(1);
            result.CommentText.ShouldBe(commentText);
        }
    }
}
