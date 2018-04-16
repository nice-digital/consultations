using System;
using System.Linq;
using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
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

            var userId = Guid.Empty;
            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            var context = new ConsultationsContext(_options, userService);
            
            var answerText = Guid.NewGuid().ToString();
            var answerId = AddAnswer(1, userId, answerText, _context);

            //Act
            var viewModel = new AnswerService(_context, userService).GetAnswer(answerId);

            //Assert
            viewModel.answer.AnswerText.ShouldBe(answerText);
        }

        [Fact]
        public void Answer_CanBeEdited()
        {
            //Arrange
            ResetDatabase();
            var answerText = Guid.NewGuid().ToString();
            var userId = Guid.Empty;

            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            var context = new ConsultationsContext(_options, userService);

            var answerId = AddAnswer(1, userId, answerText, context);

            var answerService = new AnswerService(context, userService);
            var viewModel = answerService.GetAnswer(answerId);

            var updatedAnswerText = Guid.NewGuid().ToString();
            viewModel.answer.AnswerText = updatedAnswerText;

            //Act
            var result = answerService.EditAnswer(answerId, viewModel.answer);
            viewModel = answerService.GetAnswer(answerId);

            //Assert
            result.rowsUpdated.ShouldBe(1);
            viewModel.answer.AnswerText.ShouldBe(updatedAnswerText);
        }

        [Fact]
        public void Answer_CanBeDeleted()
        {
            //Arrange
            ResetDatabase();
            var answerText = Guid.NewGuid().ToString();
            var userId = Guid.Empty;;

            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            var context = new ConsultationsContext(_options, userService);
            var answerId = AddAnswer(1, userId, answerText, context);

            var answerService = new AnswerService(context, userService);

            //Act
            var result = answerService.DeleteAnswer(answerId);
            var viewModel = answerService.GetAnswer(answerId);

            //Assert
            result.rowsUpdated.ShouldBe(1);
            viewModel.answer.ShouldBeNull();
        }

        [Fact]
        public void Answer_Record_To_Be_Deleted_Not_Found()
        {
            //Arrange
            ResetDatabase();
            var answerId = 1;
            var userId = Guid.Empty;
            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            var answerService = new AnswerService(new ConsultationsContext(_options, userService), userService);

            //Act
            var result = answerService.DeleteAnswer(answerId);

            //Assert
            result.validate.NotFound.ShouldBeTrue();
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
            var userId = Guid.Empty;

            var locationId = AddLocation(sourceURI);
            var questionTypeId = AddQuestionType(description, false, true);
            var questionId = AddQuestion(locationId, questionTypeId, questionText);

            var questionType = new QuestionType(description, false, true, null);
            var location = new Location(sourceURI, null, null, null, null, null, null, null, null);
            var question = new Question(locationId, questionText, questionTypeId, null, location, questionType, null);

            var answer = new Answer(questionId, userId, answerText, false, question);
            var viewModel = new ViewModels.Answer(answer);

            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            var answerService = new AnswerService(new ConsultationsContext(_options, userService), userService);

            //Act
            var result = answerService.CreateAnswer(viewModel);

            //Assert
            result.answer.AnswerId.ShouldBeGreaterThan(0);
            result.answer.AnswerText.ShouldBe(answerText);
            result.answer.AnswerBoolean.ShouldBe(false);
        }

        [Fact]
        public void No_Answers_returned_when_not_logged_in()
        {
            // Arrange
            ResetDatabase();
            var sourceURI = "/consultations/1/1/introduction";
            var commentText = Guid.NewGuid().ToString();
            var answerText = Guid.NewGuid().ToString();
            var questionText = Guid.NewGuid().ToString();
            var userId = Guid.Empty;

            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            var context = new ConsultationsContext(_options, userService);
            AddCommentsAndQuestionsAndAnswers(sourceURI, commentText, questionText, answerText, userId, context);

            // Act
            var viewModel = new AnswerService(new ConsultationsContext(_options, _fakeUserService), FakeUserService.Get(isAuthenticated: false)).GetAnswer(1);

            //Assert
            viewModel.validate.Unauthorised.ShouldBeTrue();
            viewModel.answer.ShouldBeNull();
        }

        [Fact]
        public void Only_own_Answers_returned_when_logged_in()
        {
            //Arrange
            ResetDatabase();
            var sourceURI = "/consultations/1/1/introduction";
            var commentText = Guid.NewGuid().ToString();
            var answerText = Guid.NewGuid().ToString();
            var questionText = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid();

            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            var context = new ConsultationsContext(_options, userService);
            AddCommentsAndQuestionsAndAnswers(sourceURI, commentText, questionText, answerText, userId, _context);

            var expectedAnswerId = AddAnswer(1, userId, "current user's answer", _context);
            var anotherPersonsAnswerId = AddAnswer(1, Guid.NewGuid(), "another user's answer", _context);
            
            var commentService = new CommentService(_context, userService);

            // Act
            var viewModel = commentService.GetCommentsAndQuestions(sourceURI);
            var questionViewModel = viewModel.Questions.SingleOrDefault(q => q.QuestionId.Equals(1));
            var myAnswer = questionViewModel.Answers.SingleOrDefault(a => a.AnswerId.Equals(expectedAnswerId));
            var otherAnswer = questionViewModel.Answers.SingleOrDefault(a => a.AnswerId.Equals(anotherPersonsAnswerId));

            //Assert
            myAnswer.AnswerId.ShouldBe(expectedAnswerId);
            otherAnswer.ShouldBeNull();
        }
    }
}
