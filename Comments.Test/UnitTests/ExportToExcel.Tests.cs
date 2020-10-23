using Comments.Configuration;
using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using NICE.Feeds;
using NICE.Feeds.Models.Indev.List;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using TestBase = Comments.Test.Infrastructure.TestBase;

namespace Comments.Test.UnitTests
{
	public class ExcelTests : TestBase
	{
		private readonly IFeedService _feedService;

		public ExcelTests()
		{
			var consultationList = new List<ConsultationList>
			{
				new ConsultationList { ConsultationId = 1, AllowedRole = TestUserType.IndevUser.ToString() }
			};
			_feedService = new FakeFeedService(consultationList);
			System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
		}

		[Fact]
		public void GetAllSubmittedCommentsForURI_OnlyReturnsOneConsultation()
		{
			//Arrange
			int locationId, submissionId, commentId;
			var userId = Guid.NewGuid().ToString();

			locationId = AddLocation("consultations://./consultation/1/document/1", _context, "001.001.000.000");
			commentId = AddComment(locationId, "Submitted comment consultation 1", userId, (int)StatusName.Submitted, _context);
			submissionId = AddSubmission(userId, _context);
			AddSubmissionComments(submissionId, commentId, _context);

			locationId = AddLocation("consultations://./consultation/154/document/1", _context, "001.001.000.000");
			commentId = AddComment(locationId, "Submitted comment consultation 154", userId, (int)StatusName.Submitted, _context);
			submissionId = AddSubmission(userId, _context);
			AddSubmissionComments(submissionId, commentId, _context);

			const string sourceURI = "consultations://./consultation/1";

			//Act
			var questions = _context.GetAllSubmittedCommentsForURI(sourceURI);

			//Assert
			questions.Count.ShouldBe(1);
		}

		[Fact]
		public void GetAllSubmittedAnswersForURI_OnlyReturnsOneConsultation()
		{
			//Arrange
			int locationId, questionId, answerId;
			var userId = Guid.NewGuid().ToString();
			var submissionId = AddSubmission(userId, _context);
			var questionTypeId = 99;

			locationId = AddLocation("consultations://./consultation/1", _context, "001.002.001.000");
			questionId = AddQuestion(locationId, questionTypeId, "Consultation Level Question for Consultation 1", _context);
			answerId = AddAnswer(questionId, userId, "answering a consultation level question for Consultation 1", (int)StatusName.Submitted, _context);
			AddSubmissionAnswers(submissionId, answerId, _context);

			locationId = AddLocation("consultations://./consultation/154", _context, "001.002.001.000");
			questionId = AddQuestion(locationId, questionTypeId, "Consultation Level Question for Consultation 154", _context);
			answerId = AddAnswer(questionId, userId, "answering a consultation level question for consultation 154", (int)StatusName.Submitted, _context);
			AddSubmissionAnswers(submissionId, answerId, _context);

			const string sourceURI = "consultations://./consultation/1";

			//Act
			var answers = _context.GetAllSubmittedAnswersForURI(sourceURI);

			//Assert
			answers.Count.ShouldBe(1);
		}

		[Fact]
		public void GetUnansweredQuestionsForURI_OnlyReturnsQuestionsForOneConsultation()
		{
			//Arrange
			int locationId;
			var questionTypeId = 99; ;
			locationId = AddLocation("consultations://./consultation/1", _context, "001.002.000.000");
			AddQuestion(locationId, questionTypeId, "Without an answer for consultation 1", _context);

			locationId = AddLocation("consultations://./consultation/154", _context, "001.002.000.000");
			AddQuestion(locationId, questionTypeId, "Without an answer for consultation 154", _context);

			const string sourceURI = "consultations://./consultation/1";

			//Act
			var questions = _context.GetUnansweredQuestionsForURI(sourceURI);

			//Assert
			questions.Count.ShouldBe(1);
		}

		[Fact]
		public void GetAllSubmittedCommentsForURI_OnlyReturnsSubmittedComments()
		{
			//Arrange
			int locationId, commentId;
			var userId = Guid.NewGuid().ToString();
			var submissionId = AddSubmission(userId, _context);

			locationId = AddLocation("consultations://./consultation/1", _context, "001.000.000.000");
			commentId = AddComment(locationId, "Submitted comment", userId, (int)StatusName.Submitted, _context);
			submissionId = AddSubmission(userId, _context);
			AddSubmissionComments(submissionId, commentId, _context);

			locationId = AddLocation("consultations://./consultation/1/document/2/chapter/introduction", _context, "001.002.002.000");
			commentId = AddComment(locationId, "Draft comment", userId, (int)StatusName.Draft, _context);
			AddSubmissionComments(submissionId, commentId, _context);
			
			locationId = AddLocation("consultations://./consultation/1/document/1/chapter/chapter-slug", _context, "001.002.000.000");
			commentId = AddComment(locationId, "Another Users Submitted comment", Guid.NewGuid().ToString(), (int)StatusName.Submitted, _context);
			AddSubmissionComments(submissionId, commentId, _context);

			const string sourceURI = "consultations://./consultation/1";

			//Act
			var comments = _context.GetAllSubmittedCommentsForURI(sourceURI);

			//Assert
			comments.Count.ShouldBe(2);
		}

		[Fact]
		public void GetAllSubmittedAnswersForURI_OnlyReturnsSubmittedAnswers()
		{
			//Arrange
			int locationId, answerId;
			var userId = Guid.NewGuid().ToString();
			var submissionId = AddSubmission(userId, _context);

			var questionTypeId = 99;
			locationId = AddLocation("consultations://./consultation/1/document/2/chapter/guidance", _context, "001.002.001.000");
			var questionId = AddQuestion(locationId, questionTypeId, "Question 1", _context);

			answerId = AddAnswer(questionId, userId, "This is a submitted answer", (int)StatusName.Submitted, _context);
			AddSubmissionAnswers(submissionId, answerId, _context);

			answerId = AddAnswer(questionId, Guid.NewGuid().ToString(), "An answer to the same question by another user", (int)StatusName.Submitted, _context);
			AddSubmissionAnswers(submissionId, answerId, _context);

			answerId = AddAnswer(questionId, userId, "This is a draft answer", (int)StatusName.Draft, _context);
			AddSubmissionAnswers(submissionId, answerId, _context);

			const string sourceURI = "consultations://./consultation/1";

			//Act
			var answers = _context.GetAllSubmittedAnswersForURI(sourceURI);

			//Assert
			answers.Count.ShouldBe(2);
		}

		[Fact]
		public void GetUsersCommentsForURI_OnlyShowsCurrentUsersComments()
		{
			//Arrange
			int locationId, commentId;
			var userId = Guid.Empty.ToString();
			var submissionId = AddSubmission(userId, _context);

			locationId = AddLocation("consultations://./consultation/1", _context, "001.000.000.000");
			commentId = AddComment(locationId, "Submitted comment", userId, (int)StatusName.Submitted, _context);
			submissionId = AddSubmission(userId, _context);
			AddSubmissionComments(submissionId, commentId, _context);

			locationId = AddLocation("consultations://./consultation/1/document/2/chapter/introduction", _context, "001.002.002.000");
			commentId = AddComment(locationId, "Draft comment", userId, (int)StatusName.Draft, _context);
			AddSubmissionComments(submissionId, commentId, _context);

			locationId = AddLocation("consultations://./consultation/1/document/1/chapter/chapter-slug", _context, "001.002.000.000");
			commentId = AddComment(locationId, "Another Users Submitted comment", Guid.NewGuid().ToString(), (int)StatusName.Submitted, _context);
			AddSubmissionComments(submissionId, commentId, _context);

			const string sourceURI = "consultations://./consultation/1";

			//Act
			var comments = _context.GetUsersCommentsForURI(sourceURI);

			//Assert
			comments.Count.ShouldBe(2);
		}

		[Fact]
		public void GetUsersAnswersForURI_OnlyShowsCurrentUsersAnswers()
		{
			//Arrange
			int locationId, answerId;
			var userId = Guid.Empty.ToString();
			var submissionId = AddSubmission(userId, _context);

			var questionTypeId = 99;
			locationId = AddLocation("consultations://./consultation/1/document/2/chapter/guidance", _context, "001.002.001.000");
			var questionId = AddQuestion(locationId, questionTypeId, "Question 1", _context);

			answerId = AddAnswer(questionId, userId, "This is a submitted answer", (int)StatusName.Submitted, _context);
			AddSubmissionAnswers(submissionId, answerId, _context);

			answerId = AddAnswer(questionId, Guid.NewGuid().ToString(), "An answer to the same question by another user", (int)StatusName.Submitted, _context);
			AddSubmissionAnswers(submissionId, answerId, _context);

			answerId = AddAnswer(questionId, userId, "This is a draft answer", (int)StatusName.Draft, _context);
			AddSubmissionAnswers(submissionId, answerId, _context);

			const string sourceURI = "consultations://./consultation/1";

			//Act
			var answers = _context.GetUsersAnswersForURI(sourceURI);

			//Assert
			answers.Count.ShouldBe(2);
		}

		[Fact]
		public void GetUsersUnansweredQuestionsForURI_OnlyShowsCurrentUnansweredQuestions()
		{
			//Arrange

			int locationId,  questionId, answerId;
			var userId = Guid.Empty.ToString();
			var submissionId = AddSubmission(userId, _context);
			var questionTypeId = 99;

			locationId = AddLocation("consultations://./consultation/1/document/2/chapter/guidance", _context, "001.002.001.000");
			questionId = AddQuestion(locationId, questionTypeId, "Question Answered ", _context);
			answerId = AddAnswer(questionId, userId, "Current Users answer", (int)StatusName.Draft, _context);
			AddSubmissionAnswers(submissionId, answerId, _context);

			locationId = AddLocation("consultations://./consultation/1/document/3/chapter/intro", _context, "001.002.001.000");
			questionId = AddQuestion(locationId, questionTypeId, "Question Answered By different user", _context);
			answerId = AddAnswer(questionId, Guid.NewGuid().ToString(), "different Users answer", (int)StatusName.Draft, _context);
			AddSubmissionAnswers(submissionId, answerId, _context);

			locationId = AddLocation("consultations://./consultation/1/document/2", _context, "001.002.000.000");
			AddQuestion(locationId, questionTypeId, "Without an answer", _context);

			const string sourceURI = "consultations://./consultation/1";

			//Act
			var questions = _context.GetUsersUnansweredQuestionsForURI(sourceURI);

			//Assert
			questions.Count.ShouldBe(2);
		}

		[Fact]
		public void GetLocationData()
		{
			// Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var userId = Guid.Empty.ToString();

			var locationId = AddLocation("consultations://./consultation/1/document/1/chapter/chapter-slug", _context, "001.000.000.000");
			var commentId = AddComment(locationId, "Submitted comment", userId, (int)StatusName.Submitted, _context);
			var submissionId = AddSubmission(userId, _context);
			AddSubmissionComments(submissionId, commentId, _context);

			const string sourceURI = "consultations://./consultation/1/document/1/chapter/chapter-slug";

			var comments = _context.GetAllSubmittedCommentsForURI(sourceURI);
			var exportService = new ExportService(_context, _fakeUserService, _consultationService, _fakeHttpContextAccessor, _feedService);

			//Act
			var locationDetails = exportService.GetLocationData(comments.First().Location);

			//Assert
			locationDetails.ConsultationName.ShouldBe("ConsultationName");
			locationDetails.DocumentName.ShouldBe("doc 1");
			locationDetails.ChapterName.ShouldBe("chapter-slug");
		}

		[Fact]
		public void GetAllDataForConsulationForCurrentUser()
		{
			// Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var userId = Guid.NewGuid().ToString();
			int locationId, submissionId, commentId, questionId, answerId;
			var questionTypeId = 99;

			//Add a comment
			locationId = AddLocation("consultations://./consultation/1/document/1", _context, "001.001.000.000");
			commentId = AddComment(locationId, "Submitted comment consultation 1", userId, (int)StatusName.Submitted, _context);
			submissionId = AddSubmission(userId, _context);
			AddSubmissionComments(submissionId, commentId, _context);

			//Add a question with an answer
			locationId = AddLocation("consultations://./consultation/1", _context, "001.002.001.000");
			questionId = AddQuestion(locationId, questionTypeId, "Consultation Level Question for Consultation 1", _context);
			answerId = AddAnswer(questionId, userId, "answering a consultation level question for Consultation 1", (int)StatusName.Submitted, _context);
			AddSubmissionAnswers(submissionId, answerId, _context);

			//Add an unanswered question
			locationId = AddLocation("consultations://./consultation/1", _context, "001.002.000.000");
			AddQuestion(locationId, questionTypeId, "Without an answer for consultation 1", _context);

			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var context = new ConsultationsContext(_options, userService, _fakeEncryption);

			var consultationService = new ConsultationService(_context, new FakeFeedService(), new FakeLogger<ConsultationService>(), _fakeUserService);
			var export = new ExportService(context, _fakeUserService, consultationService, _fakeHttpContextAccessor, _feedService);

			//Act
			var resultTuple = export.GetAllDataForConsulationForCurrentUser(1);

			//Assert
			resultTuple.comment.Count().ShouldBe(1);
			resultTuple.answer.Count().ShouldBe(1);
			resultTuple.question.Count().ShouldBe(1);
		}

		[Fact]
		public void GetAllDataForConsulation()
		{
			// Arrange
			ResetDatabase();
			AppSettings.ConsultationListConfig = TestAppSettings.GetConsultationListConfig();
			_context.Database.EnsureCreated();

			int locationId, submissionId, commentId, answerId, questionId;
			var userId = Guid.NewGuid().ToString();
			int questionTypeId = 99;

			// Add comment
			locationId = AddLocation("consultations://./consultation/1/document/1", _context, "001.001.000.000");
			commentId = AddComment(locationId, "Submitted comment consultation 1", userId, (int)StatusName.Submitted, _context);
			submissionId = AddSubmission(userId, _context);
			AddSubmissionComments(submissionId, commentId, _context);

			//Add question and answer
			locationId = AddLocation("consultations://./consultation/1/document/2/chapter/guidance", _context, "001.002.001.000");
			questionId = AddQuestion(locationId, questionTypeId, "Question 1", _context);
			answerId = AddAnswer(questionId, userId, "This is a submitted answer", (int)StatusName.Submitted, _context);
			AddSubmissionAnswers(submissionId, answerId, _context);

			//Add unanswered question
			locationId = AddLocation("consultations://./consultation/1/document/2", _context, "001.002.000.000");
			AddQuestion(locationId, questionTypeId, "Without an answer", _context);

			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			var context = new ConsultationsContext(_options, userService, _fakeEncryption);

			var consultationService = new ConsultationService(_context, new FakeFeedService(), new FakeLogger<ConsultationService>(), _fakeUserService);
			var export = new ExportService(context, _fakeUserService, consultationService, _fakeHttpContextAccessor, _feedService);

			//Act
			var resultTuple = export.GetAllDataForConsulation(1);

			//Assert
			resultTuple.comment.Count().ShouldBe(1);
			resultTuple.answer.Count().ShouldBe(1);
			resultTuple.question.Count().ShouldBe(1);
		}
	}
}
