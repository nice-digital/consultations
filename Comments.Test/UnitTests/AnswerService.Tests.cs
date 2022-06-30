using System;
using System.Linq;
using System.Threading.Tasks;
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
            viewModel.validate.Unauthenticated.ShouldBeTrue();
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
            var location = new Location(sourceURI, null, null, null, null, null, null, null, null, null, null, null);
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
            viewModel.validate.Unauthenticated.ShouldBeTrue();
            viewModel.answer.ShouldBeNull();
        }

        [Fact]
        public async Task Only_own_Answers_returned_when_logged_in()
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
			var commentService = new CommentService(context, userService, _consultationService, _fakeHttpContextAccessor);

            // Act
            var viewModel = await commentService.GetCommentsAndQuestions(sourceURI, _urlHelper);
            var questionViewModel = viewModel.Questions.SingleOrDefault(q => q.QuestionId.Equals(questionId));

            //Assert
            questionViewModel.Answers.Single().AnswerId.ShouldBe(expectedAnswerId);
        }

		[Fact]
		public void Only_return_answers_from_my_organisation_which_have_been_submitted_to_lead_in_correct_order()
		{
			// Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();

			var sessionId = Guid.NewGuid();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			const int organisationUserId = 1;
			const int organisationId = 1;
			const int otherUsersorganisationUserId = 2;
			const int differentUsersOrganisationUserID = 3;
			const int questionTypeId = 50;
			const string answerTextThatShouldBeReturned = "another user from my organisations answer submitted to lead";
			const string emailAddress = "theotherusersemail@organisation.com";
			var organisationUser = new OrganisationUser() { EmailAddress = emailAddress, OrganisationUserId = otherUsersorganisationUserId };
			var differentOrganisationUser = new OrganisationUser() { EmailAddress = "email@organisation.com", OrganisationUserId = differentUsersOrganisationUserID };

			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: null, organisationUserId: organisationUserId);
			var context = new ConsultationsContext(_options, userService, _fakeEncryption);
			var commentService = new CommentService(new ConsultationsContext(_options, userService, _fakeEncryption), userService, _consultationService, _fakeHttpContextAccessor);
			var locationId = AddLocation(sourceURI);

			AddQuestionType("Text question", hasBooleanAnswer: false, hasTextAnswer: true, questionTypeId: questionTypeId);
			var questionId = AddQuestion(locationId, questionTypeId, "Some question text");

			AddAnswer(questionId, userId: null, "current user's answer", organisationUserId: organisationUserId, organisationId: organisationId);
			AddAnswer(questionId, userId: null, "another user from my organisations answer not submitted", organisationUserId: 9999, organisationId: organisationId, status: (int)StatusName.Draft);
			AddAnswer(questionId, userId: null, "first answer submitted", organisationUserId: differentUsersOrganisationUserID, organisationId: organisationId, status: (int)StatusName.SubmittedToLead, organisationUser: differentOrganisationUser, lastModifiedDate: DateTime.MinValue);
			AddAnswer(questionId, userId: null, answerTextThatShouldBeReturned, organisationUserId: otherUsersorganisationUserId, organisationId: organisationId, status: (int)StatusName.SubmittedToLead, organisationUser: organisationUser);
			AddAnswer(questionId, userId: null, "another user from a different organisation answer", status: (int)StatusName.SubmittedToLead, organisationUserId: organisationUserId, organisationId: 2);
			AddAnswer(questionId, userId: "1", "an individual users answer", status: (int)StatusName.SubmittedToLead);

			// Act
			var viewModel = commentService.GetCommentsAndQuestionsFromOtherOrganisationCommenters("/1/1/introduction", _urlHelper);

			//Assert
			var answers = viewModel.Questions.Single().Answers;
			answers.Count.ShouldBe(2);
			answers.First().AnswerText.ShouldBe(answerTextThatShouldBeReturned);
			answers.First().CommenterEmail.ShouldBe(emailAddress);
		}
	}
}
