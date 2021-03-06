using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Comments.Services;
using Shouldly;
using Xunit;
using Comments.Models;
using Comments.Test.Infrastructure;
using DocumentFormat.OpenXml.Office2010.Word;

namespace Comments.Test.UnitTests
{
	public class QuestionServiceTests : Infrastructure.TestBase
	{
		[Fact]
		public void Question_Get()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var sourceUri = "consultations://./consultation/1/document/1/chapter/introduction";
			var description = Guid.NewGuid().ToString();
			var questionText = Guid.NewGuid().ToString();
			var userId = Guid.Empty.ToString();

			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);

			var locationId = AddLocation(sourceUri);
			var questionTypeId = 99;
			var questionId = AddQuestion(locationId, questionTypeId, questionText);


			var questionService =
				new QuestionService(new ConsultationsContext(_options, userService, _fakeEncryption), userService, _consultationService, null, null);

			//Act
			var viewModel = questionService.GetQuestion(questionId);

			//Assert
			viewModel.question.QuestionText.ShouldBe(questionText);
		}

		[Fact]
		public void Question_Get_Record_Not_Found()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();

			var userId = Guid.Empty.ToString();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);

			var questionService =
				new QuestionService(new ConsultationsContext(_options, userService, _fakeEncryption), userService, _consultationService, null, null);

			//Act
			var viewModel = questionService.GetQuestion(1);

			//Assert
			viewModel.validate.NotFound.ShouldBeTrue();
			viewModel.question.ShouldBeNull();
		}

		[Fact]
		public void Question_CanBeEdited()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var description = Guid.NewGuid().ToString();
			var questionText = Guid.NewGuid().ToString();
			var userId = Guid.Empty.ToString();

			var locationId = AddLocation(sourceURI);
			var questionTypeId = 99;
			var questionId = AddQuestion(locationId, questionTypeId, questionText);
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var questionService =
				new QuestionService(new ConsultationsContext(_options, userService, _fakeEncryption), userService, _consultationService, null, null);

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
			_context.Database.EnsureCreated();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var description = Guid.NewGuid().ToString();
			var questionText = Guid.NewGuid().ToString();
			var userId = Guid.Empty.ToString();

			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);

			var locationId = AddLocation(sourceURI);
			var questionTypeId = 99;
			var questionId = AddQuestion(locationId, questionTypeId, questionText);

			var questionService =
				new QuestionService(new ConsultationsContext(_options, userService, _fakeEncryption), userService, _consultationService, null, null);

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
			_context.Database.EnsureCreated();
			var questionId = 1;
			var userId = Guid.Empty.ToString();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var questionService =
				new QuestionService(new ConsultationsContext(_options, userService, _fakeEncryption), userService, _consultationService, null, null);

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
			_context.Database.EnsureCreated();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var description = Guid.NewGuid().ToString();
			var questionText = Guid.NewGuid().ToString();
			var userId = Guid.Empty.ToString();

			var locationId = AddLocation(sourceURI);
			var questionTypeId = 99;
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var questionService =
				new QuestionService(new ConsultationsContext(_options, userService, _fakeEncryption), userService, _consultationService, null, null);

			var location = new Location(sourceURI, null, null, null, null, null, null, null, null, null, null, null);
			var questionType = new QuestionType(description, false, true, null);
			var question = new Question(locationId, questionText, questionTypeId, null, questionType, new List<Answer>());
			var viewModel = new ViewModels.Question(location, question);

			//Act
			var result = questionService.CreateQuestion(viewModel);

			//Assert
			result.question.QuestionText.ShouldBe(questionText);
		}

		[Fact]
		public async void PopulateQuestionAdmin()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var questionTypeId = 99;

			var consultationLevelLocationId = AddLocation("consultations://./consultation/1");
			AddQuestion(consultationLevelLocationId, questionTypeId, "Question Label");

			var documentLevelLocationId = AddLocation("consultations://./consultation/1/document/1");
			AddQuestion(documentLevelLocationId, questionTypeId, "Document Question Label");

			var userId = Guid.Empty.ToString();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var questionService = new QuestionService(new ConsultationsContext(_options, userService, _fakeEncryption), userService, _consultationService, _fakeHttpContextAccessor, _fakeApiService);

			//Act
			var result = await questionService.GetQuestionAdmin(1, false, null);

			//Assert
			result.ConsultationTitle.ShouldBe("Consultation Title");
			result.Documents.Count().ShouldBe(1);
			result.Documents.First().DocumentQuestions.Count().ShouldBe(1);
			result.Documents.First().DocumentQuestions.First().QuestionText.ShouldBe("Document Question Label");
			result.Documents.First().Title.ShouldBe("doc 1");

			result.ConsultationQuestions.Count().ShouldBe(1);
			result.ConsultationQuestions.First().QuestionText.ShouldBe("Question Label");
		}

		[Fact]
		public void Question_LastModified_Has_Changed()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var description = Guid.NewGuid().ToString();
			var questionText = Guid.NewGuid().ToString();
			var userId = Guid.NewGuid().ToString();

			var locationId = AddLocation(sourceURI);
			var questionTypeId = 99;
			var questionId = AddQuestion(locationId, questionTypeId, questionText);
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var questionService =
				new QuestionService(new ConsultationsContext(_options, userService, _fakeEncryption), userService, _consultationService, null, null);

			var viewModel = questionService.GetQuestion(questionId);
			var updatedQuestionText = Guid.NewGuid().ToString();

			viewModel.question.QuestionText = updatedQuestionText;

			var lastModifiedDate = viewModel.question.LastModifiedDate;
			var lastModifiedByUserId = viewModel.question.LastModifiedByUserId;

			//Act
			var result = questionService.EditQuestion(questionId, viewModel.question);
			var updatedViewModel = questionService.GetQuestion(questionId);

			//Assert
			result.rowsUpdated.ShouldBe(1);
			updatedViewModel.question.LastModifiedDate.ShouldNotBe(lastModifiedDate);
			updatedViewModel.question.LastModifiedByUserId.ShouldNotBe(lastModifiedByUserId);
		}

		[Fact]
		public void Get_QuestionTypes()
		{
			//Arrange
			ResetDatabase();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: Guid.NewGuid().ToString());
			var questionService =
				new QuestionService(new ConsultationsContext(_options, userService, _fakeEncryption), userService, _consultationService, null, null);

			var textQuestionTypeId = AddQuestionType("Text", hasBooleanAnswer: false, hasTextAnswer: true, questionTypeId: 1);
			var booleanQuestionTypeId = AddQuestionType("Boolean", hasBooleanAnswer: true, hasTextAnswer: false, questionTypeId: 2);

			//Act
			IEnumerable<ViewModels.QuestionType> results = questionService.GetQuestionTypes();

			//Assert
			results.Count().ShouldBe(2);
			results.First().QuestionTypeId.ShouldBe(textQuestionTypeId);
			results.Skip(1).First().QuestionTypeId.ShouldBe(booleanQuestionTypeId);
		}
	}
}
