using System;
using System.Collections.Generic;
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
			var sourceUri = "consultations://./consultation/1/document/1/chapter/introduction";
			var description = Guid.NewGuid().ToString();
			var questionText = Guid.NewGuid().ToString();
			var userId = Guid.Empty;

			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);

			var locationId = AddLocation(sourceUri);
			var questionTypeId = AddQuestionType(description, false, true, 1);
			var questionId = AddQuestion(locationId, questionTypeId, questionText);


			var questionService =
				new QuestionService(new ConsultationsContext(_options, userService, _fakeEncryption), userService);

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

			var userId = Guid.Empty;
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);

			var questionService =
				new QuestionService(new ConsultationsContext(_options, userService, _fakeEncryption), userService);

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
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var description = Guid.NewGuid().ToString();
			var questionText = Guid.NewGuid().ToString();
			var userId = Guid.Empty;

			var locationId = AddLocation(sourceURI);
			var questionTypeId = AddQuestionType(description, false, true);
			var questionId = AddQuestion(locationId, questionTypeId, questionText);
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var questionService =
				new QuestionService(new ConsultationsContext(_options, userService, _fakeEncryption), userService);

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
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var description = Guid.NewGuid().ToString();
			var questionText = Guid.NewGuid().ToString();
			var userId = Guid.Empty;

			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);

			var locationId = AddLocation(sourceURI);
			var questionTypeId = AddQuestionType(description, false, true, 1);
			var questionId = AddQuestion(locationId, questionTypeId, questionText);

			var questionService =
				new QuestionService(new ConsultationsContext(_options, userService, _fakeEncryption), userService);

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
			var questionService =
				new QuestionService(new ConsultationsContext(_options, userService, _fakeEncryption), userService);

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
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var description = Guid.NewGuid().ToString();
			var questionText = Guid.NewGuid().ToString();
			var userId = Guid.Empty;

			var locationId = AddLocation(sourceURI);
			var questionTypeId = AddQuestionType(description, false, true);
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var questionService =
				new QuestionService(new ConsultationsContext(_options, userService, _fakeEncryption), userService);

			var location = new Location(sourceURI, null, null, null, null, null, null, null, null, null, null);
			var questionType = new QuestionType(description, false, true, null);
			var question = new Question(locationId, questionText, questionTypeId, null, questionType, new List<Answer>());
			var viewModel = new ViewModels.Question(location, question);

			//Act
			var result = questionService.CreateQuestion(viewModel);

			//Assert
			result.question.QuestionText.ShouldBe(questionText);
		}

		//  [Fact]
		//  public void GetQuestionsByConsulationId()
		//  {
		////Arrange
		//   var questionTypeId = AddQuestionType("Question Type", false, true);

		//var consultationLevelLocationId = AddLocation("consultations://./consultation/1");
		//var questionIdToBeReturned = AddQuestion(consultationLevelLocationId, questionTypeId, "Question Label");

		//   var differentConsultationLevelLocationId = AddLocation("consultations://./consultation/2");
		//   var questionIdForDifferentConsultation = AddQuestion(differentConsultationLevelLocationId, questionTypeId, "Question Label");

		//var userId = Guid.Empty;
		//var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
		//var questionService = new QuestionService(new ConsultationsContext(_options, userService, _fakeEncryption), userService);

		////Act
		//   var result = questionService.GetQuestions(1, null);

		//   //Assert
		//result.Count().ShouldBe(1);
		//   result.First().QuestionId.ShouldBe(questionIdToBeReturned);
		//  }

		//  [Fact]
		//  public void GetQuestionsByDocumentId()
		//  {
		////Arrange
		//var questionTypeId = AddQuestionType("Question Type", false, true);

		//var documentLevelLocationId = AddLocation("consultations://./consultation/1/document/1");
		//   var questionIdToBeReturned = AddQuestion(documentLevelLocationId, questionTypeId, "Question Label");

		//   var differentDocumentLevelLocationId = AddLocation("consultations://./consultation/1/document/2");
		//   var questionIdforDifferentDocument = AddQuestion(differentDocumentLevelLocationId, questionTypeId, "Question Label");

		//var userId = Guid.Empty;
		//   var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
		//   var questionService = new QuestionService(new ConsultationsContext(_options, userService, _fakeEncryption), userService);

		//   //Act
		//   var result = questionService.GetQuestions(1, 1);

		//   //Assert
		//result.Count().ShouldBe(1);
		//   result.First().QuestionId.ShouldBe(questionIdToBeReturned);
		//  }

		[Fact]
		public void PopulateQuestionAdmin()
		{
			//Arrange
			ResetDatabase();
			var questionTypeId = AddQuestionType("Question Type", false, true);

			var consultationLevelLocationId = AddLocation("consultations://./consultation/1");
			var questionIdToBeReturned = AddQuestion(consultationLevelLocationId, questionTypeId, "Question Label");

			var DocumentLevelLocationId = AddLocation("consultations://./consultation/1/document/2");
			var DocumentQuestionIdToBeReturned = AddQuestion(consultationLevelLocationId, questionTypeId, "Document Question Label");

			var userId = Guid.Empty;
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var consultationService = new FakeConsultationService();
			var questionService = new QuestionService(new ConsultationsContext(_options, userService, _fakeEncryption), userService, consultationService);

			//Act
			var result = questionService.GetQuestionAdmin(1);

			//Assert
			result.ConsultationTitle.ShouldBe("Consultation Title");

			result.Breadcrumbs.Count().ShouldBe(2);
			result.Breadcrumbs.First().Label.ShouldBe("Back to In Development");
			result.Breadcrumbs.Last().Label.ShouldBe("Add question");

			result.Documents.Count().ShouldBe(1);
			result.Documents.First().DocumentQuestions.Count().ShouldBe(1);
			result.Documents.First().DocumentQuestions.First().QuestionText.ShouldBe("Document Question Label");
			result.Documents.First().Title.ShouldBe("doc 1");

			result.ConsultationQuestions.Count().ShouldBe(1);
			result.ConsultationQuestions.First().QuestionText.ShouldBe("Question Label");
		}
}
}
