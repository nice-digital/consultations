using System;
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
    public class CommentServiceTests : TestBase
    {
        [Fact]
        public void No_Comments_returned_when_not_logged_in()
        {
            // Arrange
            ResetDatabase();
            var commentService = new CommentService(new ConsultationsContext(_options), FakeUserService.Get(isAuthenticated: false));

            // Act
            var viewModel = commentService.GetCommentsAndQuestions("/consultations/1/1/introduction");

            //Assert
            viewModel.IsAuthenticated.ShouldBeFalse();
            viewModel.Comments.Count().ShouldBe(0);
            viewModel.Questions.Count().ShouldBe(0);
        }
    }
}

