using System;
using System.Linq;
using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Shouldly;
using Xunit;
using Answer = Comments.Models.Answer;
using Location = Comments.Models.Location;
using Question = Comments.Models.Question;
using QuestionType = Comments.Models.QuestionType;
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
            _context.Database.EnsureCreated();

            var answerText = Guid.NewGuid().ToString();
            var userId = Guid.Empty.ToString();
            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);

            SetupTestDataInDB();

            var question = GetQuestion();
            var answerId = AddAnswer(question.QuestionId, userId, answerText);

            //Act
            var viewModel = new AnswerService(new ConsultationsContext(_options, userService, _fakeEncryption), userService).GetAnswer(answerId);

            //Assert
            viewModel.answer.AnswerText.ShouldBe(answerText);
        }

        [Fact]
        public void Answer_Get_Record_Not_Found()
        {
            //Arrange
            ResetDatabase();

            var userId = Guid.Empty.ToString();
            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);

            var answerService = new AnswerService(new ConsultationsContext(_options, userService, _fakeEncryption), userService);

            //Act
            var viewModel = answerService.GetAnswer(1);

            //Assert
            viewModel.validate.NotFound.ShouldBeTrue();
            viewModel.answer.ShouldBeNull();
        }

        [Fact]
        public void Answer_Get_Record_Not_Logged_In()
        {
            //Arrange
            ResetDatabase();

            var userService = FakeUserService.Get(isAuthenticated: false);

            //Act
            var viewModel = new AnswerService(new ConsultationsContext(_options, userService, _fakeEncryption), userService).GetAnswer(1);

            //Assert
            viewModel.validate.Unauthorised.ShouldBeTrue();
        }

        [Fact]
        public void Answer_CanBeEdited()
        {
            //Arrange
            ResetDatabase();
	        _context.Database.EnsureCreated();

			var answerText = Guid.NewGuid().ToString();
            var userId = Guid.Empty.ToString();
            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);

            SetupTestDataInDB();

            var question = GetQuestion();
            var answerId = AddAnswer(question.QuestionId, userId, answerText);

            var answerService = new AnswerService(new ConsultationsContext(_options, userService, _fakeEncryption), userService);
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
	        _context.Database.EnsureCreated();

			var answerText = Guid.NewGuid().ToString();
            var userId = Guid.Empty.ToString();
            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);

            SetupTestDataInDB();

            var question = GetQuestion();
            var answerId = AddAnswer(question.QuestionId, userId, answerText);

            var answerService = new AnswerService(new ConsultationsContext(_options, userService, _fakeEncryption), userService);

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
            var userId = Guid.Empty.ToString();
            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            var answerService = new AnswerService(new ConsultationsContext(_options, userService, _fakeEncryption), userService);

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
            var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
            var answerText = Guid.NewGuid().ToString();
            var questionText = Guid.NewGuid().ToString();
            var description = Guid.NewGuid().ToString();
            var userId = Guid.Empty.ToString();

	        AddStatus(StatusName.Draft.ToString(), (int)StatusName.Draft);
			var locationId = AddLocation(sourceURI);
            var questionTypeId = AddQuestionType(description, false, true);
            var questionId = AddQuestion(locationId, questionTypeId, questionText);

            var questionType = new QuestionType(description, false, true, null);
            var location = new Location(sourceURI, null, null, null, null, null, null, null, null, null, null);
            var question = new Question(locationId, questionText, questionTypeId, location, questionType, null);

            var answer = new Answer(questionId, userId, answerText, false, question, (int)StatusName.Draft, null);
            var viewModel = new ViewModels.Answer(answer);

            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            var answerService = new AnswerService(new ConsultationsContext(_options, userService, _fakeEncryption), userService);

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
            var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
            var commentText = Guid.NewGuid().ToString();
            var answerText = Guid.NewGuid().ToString();
            var questionText = Guid.NewGuid().ToString();
            var userId = Guid.Empty.ToString();

            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            AddCommentsAndQuestionsAndAnswers(sourceURI, commentText, questionText, answerText, userId);

            // Act
            var viewModel = new AnswerService(new ConsultationsContext(_options, _fakeUserService, _fakeEncryption), FakeUserService.Get(isAuthenticated: false)).GetAnswer(1);

            //Assert
            viewModel.validate.Unauthorised.ShouldBeTrue();
            viewModel.answer.ShouldBeNull();
        }

        [Fact]
        public void Only_own_Answers_returned_when_logged_in()
        {
            //Arrange
            ResetDatabase();
			_context.Database.EnsureCreated();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
            var questionText = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid().ToString();

            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);

            var locationId = AddLocation(sourceURI);
			var questionTypeId = 99;
            var questionId = AddQuestion(locationId, questionTypeId, questionText);
            var expectedAnswerId = AddAnswer(questionId, userId, "current user's answer");
            AddAnswer(questionId, Guid.NewGuid().ToString(), "another user's answer");

	        var context = new ConsultationsContext(_options, userService, _fakeEncryption);
			//var submitService = new SubmitService(context, userService, _consultationService);
			var commentService = new CommentService(context, userService, _consultationService, _linkGenerator, _fakeHttpContextAccessor);

            // Act
            var viewModel = commentService.GetCommentsAndQuestions(sourceURI);
            var questionViewModel = viewModel.Questions.SingleOrDefault(q => q.QuestionId.Equals(questionId));

            //Assert
            questionViewModel.Answers.Single().AnswerId.ShouldBe(expectedAnswerId);
        }
    }
}
