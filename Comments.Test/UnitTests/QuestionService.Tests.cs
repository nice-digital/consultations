using System;
using System.Collections.Generic;
using Comments.Services;
using Shouldly;
using Xunit;
using Comments.Models;
using Comments.Test.Infrastructure;

namespace Comments.Test.UnitTests
{
    public class QuestionServiceTests : Infrastructure.TestBase
    {
        [Fact]
        public void Question_Get()
        {
            //Arrange
            ResetDatabase();
            var sourceUri = "/consultations/1/1/introduction";
            var description = Guid.NewGuid().ToString();
            var questionText = Guid.NewGuid().ToString();
            var userId = Guid.Empty;

            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);

            var locationId = AddLocation(sourceUri);
            var questionTypeId = AddQuestionType(description, false, true, 1);
            var questionId = AddQuestion(locationId, questionTypeId, questionText);

            
            var questionService = new QuestionService(new ConsultationsContext(_options, userService), userService);

            //Act
            var viewModel = questionService.GetQuestion(questionId);

            //Assert
            viewModel.question.QuestionText.ShouldBe(questionText);
        }

        [Fact]
        public void Question_CanBeEdited()
        {
            //Arrange
            ResetDatabase();
            var sourceURI = "/consultations/1/1/introduction";
            var description = Guid.NewGuid().ToString();
            var questionText = Guid.NewGuid().ToString();
            var userId = Guid.Empty;

            var locationId = AddLocation(sourceURI);
            var questionTypeId = AddQuestionType(description, false, true);
            var questionId = AddQuestion(locationId, questionTypeId, questionText);
            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            var questionService = new QuestionService(new ConsultationsContext(_options, userService), userService);

            var viewModel = questionService.GetQuestion(questionId);
            var updatedQuestionText = Guid.NewGuid().ToString();

            viewModel.question.QuestionText = updatedQuestionText;

            //Act
            var result = questionService.EditQuestion(questionId, viewModel.question);
            viewModel = questionService.GetQuestion(questionId);

            //Assert
            result.rowsUpdated.ShouldBe(1);
            viewModel.question.QuestionText.ShouldBe(updatedQuestionText);
        }

        [Fact]
        public void Question_CanBeDeleted()
        {
            //Arrange
            ResetDatabase();
            var sourceURI = "/consultations/1/1/introduction";
            var description = Guid.NewGuid().ToString();
            var questionText = Guid.NewGuid().ToString();
            var userId = Guid.Empty;

            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);

            var locationId = AddLocation(sourceURI);
            var questionTypeId = AddQuestionType(description, false, true, 1);
            var questionId = AddQuestion(locationId, questionTypeId, questionText);
            
            var questionService = new QuestionService(new ConsultationsContext(_options, userService), userService);

            //Act
            var result = questionService.DeleteQuestion(questionId);
            var viewModel = questionService.GetQuestion(questionId);

            //Assert
            result.rowsUpdated.ShouldBe(1);
            viewModel.question.ShouldBeNull();
            viewModel.validate.NotFound.ShouldBeTrue();
        }

        [Fact]
        public void Question_Record_To_Be_Deleted_Not_Found()
        {
            //Arrange
            ResetDatabase();
            var questionId = 1;
            var userId = Guid.Empty;
            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            var questionService = new QuestionService(new ConsultationsContext(_options, userService), userService);

            //Act
            var result = questionService.DeleteQuestion(questionId);

            //Assert
            result.validate.NotFound.ShouldBeTrue();
        }

        [Fact]
        public void Question_CanBeCreated()
        {
            //Arrange
            ResetDatabase();
            var sourceURI = "/consultations/1/1/introduction";
            var description = Guid.NewGuid().ToString();
            var questionText = Guid.NewGuid().ToString();
            var userId = Guid.Empty;

            var locationId = AddLocation(sourceURI);
            var questionTypeId = AddQuestionType(description, false, true);
            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            var questionService = new QuestionService(new ConsultationsContext(_options, userService), userService);

            var location = new Location(sourceURI, null, null, null, null, null, null, null, null);
            var questionType = new QuestionType(description, false, true, null);
            var question = new Question(locationId, questionText, questionTypeId, null, null, questionType, new List<Answer>());
            var viewModel = new ViewModels.Question(location, question);

            //Act
            var result = questionService.CreateQuestion(viewModel);

            //Assert
            result.question.QuestionText.ShouldBe(questionText);
        }
    }
}
