﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Shouldly;
using Xunit;

namespace Comments.Test.UnitTests
{
    public class CommentServiceTests1 : TestBase
    {
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

