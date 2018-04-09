
using System;
using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using NICE.Feeds;
using NICE.Feeds.Tests.Infrastructure;
using Shouldly;
using Xunit;
using TestBase = Comments.Test.Infrastructure.TestBase;

namespace Comments.Test.UnitTests
{
    public class AnswerServiceTests : TestBase
    {
        [Fact]
        public void Answer_Get()
        {
            //Arrange
            ResetDatabase();
            var sourceURI = "/consultations/1/1/introduction";
            var answerText = Guid.NewGuid().ToString();
            var questionText = Guid.NewGuid().ToString();
            var description = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid();

            var locationId = AddLocation(sourceURI);
            var questionType = AddQuestionType(description, false, true);
            var questionId = AddQuestion(locationId, questionType, questionText);
            var answerId = AddAnswer(questionId, userId, answerText);

            var answerService = new AnswerService(new ConsultationsContext(_options));

            //Act
            var viewModel = answerService.GetAnswer(answerId);

            //Assert
            viewModel.AnswerText.ShouldBe(answerText);
        }

        [Fact]
        public void Answer_CanBeEdited()
        {
            //Arrange
            ResetDatabase();
            var sourceURI = "/consultations/1/1/introduction";
            var answerText = Guid.NewGuid().ToString();
            var questionText = Guid.NewGuid().ToString();
            var description = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid();

            var locationId = AddLocation(sourceURI);
            var questionType = AddQuestionType(description, false, true);
            var questionId = AddQuestion(locationId, questionType, questionText);
            var answerId = AddAnswer(questionId, userId, answerText);

            var answerService = new AnswerService(new ConsultationsContext(_options));
            var viewModel = answerService.GetAnswer(answerId);

            var updatedAnswerText = Guid.NewGuid().ToString();
            viewModel.AnswerText = updatedAnswerText;

            //Act
            var result = answerService.EditAnswer(answerId, viewModel);
            viewModel = answerService.GetAnswer(answerId);

            //Assert
            result.ShouldBe(1);
            viewModel.AnswerText.ShouldBe(updatedAnswerText);
        }

        [Fact]
        public void Answer_CanBeDeleted()
        {
            //Arrange
            ResetDatabase();
            var sourceURI = "/consultations/1/1/introduction";
            var answerText = Guid.NewGuid().ToString();
            var questionText = Guid.NewGuid().ToString();
            var description = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid();

            var locationId = AddLocation(sourceURI);
            var questionType = AddQuestionType(description, false, true);
            var questionId = AddQuestion(locationId, questionType, questionText);
            var answerId = AddAnswer(questionId, userId, answerText);
            var answerService = new AnswerService(new ConsultationsContext(_options));

            //Act
            var result = answerService.DeleteAnswer(answerId);
            var viewModel = answerService.GetAnswer(answerId);

            //Assert
            result.ShouldBe(1);
            viewModel.ShouldBeNull();
        }

        [Fact]
        public void Answer_Record_To_Be_Deleted_Not_Found()
        {
            //Arrange
            ResetDatabase();
            var answerId = 1;
            var answerService = new AnswerService(new ConsultationsContext(_options));

            //Act
            var result = answerService.DeleteAnswer(answerId);

            //Assert
            result.ShouldBe(0);
        }

        [Fact]
        public void Answer_CanBeCreated()
        {
            //Arrange
            ResetDatabase();
            var sourceURI = "/consultations/1/1/introduction";
            var answerText = Guid.NewGuid().ToString();
            var questionText = Guid.NewGuid().ToString();
            var description = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid();

            var locationId = AddLocation(sourceURI);
            var questionTypeId = AddQuestionType(description, false, true);
            var questionId = AddQuestion(locationId, questionTypeId, questionText);

            var questionType = new QuestionType(description, false, true, null);
            var location = new Location(sourceURI, null, null, null, null, null, null, null, null);
            var question = new Question(locationId, questionText, questionTypeId, null, location, questionType, null);

            var answer = new Answer(questionId, userId, answerText, false, question);
            var viewModel = new ViewModels.Answer(answer);

            var answerService = new AnswerService(new ConsultationsContext(_options));

            //Act
            var result = answerService.CreateAnswer(viewModel, questionId);

            //Assert
            result.AnswerId.ShouldBe(1);
            result.AnswerText.ShouldBe(answerText);
            result.AnswerBoolean.ShouldBe(false);
        }
    }
}
