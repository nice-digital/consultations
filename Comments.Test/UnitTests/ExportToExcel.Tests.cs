using Comments.Configuration;
using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comments.Export;
using Xunit;
using TestBase = Comments.Test.Infrastructure.TestBase;
using ExcelDataReader;
using Comments.Common;
using NICE.Feeds.Indev;
using NICE.Feeds.Indev.Models.List;
using Location = Comments.Models.Location;
using Submission = Comments.Models.Submission;

namespace Comments.Test.UnitTests
{
	public class ExcelTests : TestBase
	{
		private readonly IIndevFeedService _feedService;

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

			locationId = AddLocation("consultations://./consultation/1/document/2/chapter/introduction", _context, "001.002.002.000");
			commentId = AddComment(locationId, "Comment submitted to an organisational lead", userId, (int)StatusName.SubmittedToLead, _context);
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

			answerId = AddAnswer(questionId, userId, "This is an answer submitted to an organisational lead", (int)StatusName.SubmittedToLead, _context);
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
		public async Task GetLocationData()
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
			var exportService = new ExportService(_context, _fakeUserService, _consultationService, _feedService);

			//Act
			var locationDetails = await exportService.GetLocationData(comments.First().Location);

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
			var export = new ExportService(context, _fakeUserService, consultationService, _feedService);

			//Act
			var resultTuple = export.GetAllDataForConsultationForCurrentUser(1);

			//Assert
			resultTuple.comment.Count().ShouldBe(1);
			resultTuple.answer.Count().ShouldBe(1);
			resultTuple.question.Count().ShouldBe(1);
		}

		[Fact]
		public void GetAllDataForConsulationForOrganisationCodeUser()
		{
			// Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			string userId = null;
			var organisationUserId = TestBaseDBHelpers.AddOrganisationUser(_context, 1, Guid.NewGuid(), DateTime.MaxValue);
			int locationId, submissionId, commentId, questionId, answerId;
			var questionTypeId = 99;

			//Add a comment
			locationId = AddLocation("consultations://./consultation/1/document/1", _context, "001.001.000.000");
			commentId = AddComment(locationId, "Submitted comment consultation 1", userId, (int)StatusName.Submitted, _context, organisationUserId); //TODO Needs to be updated with submit to lead
			submissionId = AddSubmission(userId, _context);
			AddSubmissionComments(submissionId, commentId, _context);

			//Add a question with an answer
			locationId = AddLocation("consultations://./consultation/1", _context, "001.002.001.000");
			questionId = AddQuestion(locationId, questionTypeId, "Consultation Level Question for Consultation 1", _context);
			answerId = AddAnswer(questionId, userId, "answering a consultation level question for Consultation 1", (int)StatusName.Submitted, _context, organisationUserId); //TODO Needs to be updated with submit to lead
			AddSubmissionAnswers(submissionId, answerId, _context);

			//Add an unanswered question
			locationId = AddLocation("consultations://./consultation/1", _context, "001.002.000.000");
			AddQuestion(locationId, questionTypeId, "Without an answer for consultation 1", _context);

			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId, organisationUserId: organisationUserId);
			var context = new ConsultationsContext(_options, userService, _fakeEncryption);

			var consultationService = new ConsultationService(_context, new FakeFeedService(), new FakeLogger<ConsultationService>(), _fakeUserService);
			var export = new ExportService(context, _fakeUserService, consultationService, _feedService);

			//Act
			var resultTuple = export.GetAllDataForConsultationForCurrentUser(1);

			//Assert
			resultTuple.comment.Count().ShouldBe(1);
			resultTuple.answer.Count().ShouldBe(1);
			resultTuple.question.Count().ShouldBe(1);
		}

		[Fact]
		public async void CreateSpreadsheetForOrganisationCodeUser()
		{
			//Arrange
			const int organisationUserId = 1;
			var userService = FakeUserService.Get(isAuthenticated: false, displayName: null, userId: null, organisationUserId: organisationUserId);
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var locationId = AddLocation(sourceURI, _context, "001.001.000.000");
			var location = new Location(sourceURI, null, null, null, null, null, null, null, null, null, null);
			var commentText = "A comment";
			var comments = new List<Models.Comment> { new Models.Comment(locationId, null, commentText, null, location, 3, null, organisationUserId: organisationUserId) };
			comments.First().SubmissionComment.Add(new SubmissionComment(1, 1));
			comments.First().SubmissionComment.First().Submission = new Submission(null, DateTime.Now, false, "organisation", false, null, null);
			var answers = new List<Models.Answer> { };
			var questions = new List<Models.Question> { };
			var fakeExportService = new FakeExportService(comments, answers, questions,
				new List<OrganisationUser> { new OrganisationUser(1, Guid.NewGuid(), DateTime.Now) { EmailAddress = "bob@bob.com", OrganisationUserId = organisationUserId } });
			var exportToExcel = new ExportToExcel(userService, fakeExportService, null);

			//Act
			var spreadsheet = await exportToExcel.ToSpreadsheet(comments, answers, questions);

			using (var reader = ExcelReaderFactory.CreateReader(spreadsheet))
			{
				var workSheet = reader.AsDataSet();
				var data = workSheet.Tables;

				//Assert
				var rows = data[Constants.Export.SheetName].Rows;
				rows.Count.ShouldBe(4);

				var headerRow = rows[2].ItemArray;
				headerRow.Length.ShouldBe(15);

				headerRow[1].ToString().ShouldBe("Email Address");

				var commentRow = rows[3].ItemArray;
				commentRow[7].ShouldBe(commentText);
				commentRow[1].ShouldBe("bob@bob.com");
			}
		}

		[Fact]
		public async void CreateSpreadsheetForOrganisationLeadWithResponsesTheySubmittedToNICE()
		{
			//Arrange
			AppSettings.ConsultationListConfig = TestAppSettings.GetConsultationListConfig();
			const int organisationUserId = 1;
			const string userId = "1";
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Sarah Jane Smith", userId: userId, organisationIdUserIsLeadOf: 1, emailAddress: "sarah@tardis.gov");
			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var locationId = AddLocation(sourceURI, _context, "001.001.000.000");
			var location = new Location(sourceURI, null, null, null, null, null, null, null, null, null, null);
			var commentText = "A comment";
			var comments = new List<Models.Comment> { new Models.Comment(locationId, null, commentText, null, location, 2, null, organisationUserId: organisationUserId, parentCommentId: 1) };
			comments.First().SubmissionComment.Add(new SubmissionComment(1, 1));
			comments.First().SubmissionComment.First().Submission = new Submission(submissionByUserId: userId, DateTime.Now, true, "organisation", false, null, null);
			var answers = new List<Models.Answer> { };
			var questions = new List<Models.Question> { };
			var fakeExportService = new FakeExportService(comments, answers, questions,
				new List<OrganisationUser> { new OrganisationUser(1, Guid.NewGuid(), DateTime.Now) { EmailAddress = "sarah@tardis.gov", OrganisationUserId = organisationUserId } });
			var exportToExcel = new ExportToExcel(userService, fakeExportService, null);

			//Act
			var spreadsheet = await exportToExcel.ToSpreadsheet(comments, answers, questions);

			using (var reader = ExcelReaderFactory.CreateReader(spreadsheet))
			{
				var workSheet = reader.AsDataSet();
				var data = workSheet.Tables;

				//Assert
				var rows = data[Constants.Export.SheetName].Rows;
				rows.Count.ShouldBe(4);

				var headerRow = rows[2].ItemArray;
				headerRow.Length.ShouldBe(15);

				headerRow[1].ToString().ShouldBe("Email Address");

				var commentRow = rows[3].ItemArray;
				commentRow[7].ShouldBe(commentText);
				commentRow[1].ShouldBe("sarah@tardis.gov");
			}
		}

		[Fact]
		public async Task GetAllDataForConsulation()
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
			var export = new ExportService(context, _fakeUserService, consultationService, _feedService);

			//Act
			var resultTuple = await export.GetAllDataForConsultation(1);

			//Assert
			resultTuple.comment.Count().ShouldBe(1);
			resultTuple.answer.Count().ShouldBe(1);
			resultTuple.question.Count().ShouldBe(1);
		}

		[Fact]
		public void GetAllSubmittedToLeadCommentsForURI_OnlyReturnsSubmittedToLeadComments()
		{
			//Arrange
			int locationId, commentId;
			var userId = Guid.NewGuid().ToString();
			var submissionId = AddSubmission(userId, _context);
			var organisationUserId = 1;
			var organisationId = 1;

			locationId = AddLocation("consultations://./consultation/1", _context, "001.000.000.000");
			commentId = AddComment(locationId, "Submitted comment", userId, (int)StatusName.Submitted, _context);
			submissionId = AddSubmission(userId, _context);
			AddSubmissionComments(submissionId, commentId, _context);

			locationId = AddLocation("consultations://./consultation/1/document/2/chapter/introduction", _context, "001.002.002.000");
			commentId = AddComment(locationId, "Draft comment", null, (int)StatusName.Draft, _context, organisationUserId, null, organisationId);
			AddSubmissionComments(submissionId, commentId, _context);

			locationId = AddLocation("consultations://./consultation/1/document/2/chapter/introduction", _context, "001.002.002.000");
			commentId = AddComment(locationId, "Comment submitted to an organisational lead", null, (int)StatusName.SubmittedToLead, _context, organisationUserId, null, organisationId);
			AddSubmissionComments(submissionId, commentId, _context);

			const string sourceURI = "consultations://./consultation/1";
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Rose Tyler", userId: Guid.NewGuid().ToString(), organisationIdUserIsLeadOf: organisationId);
			var context = new ConsultationsContext(_options, userService, _fakeEncryption);

			//Act
			var comments = context.GetCommentsAndAnswersSubmittedToALeadForURI(sourceURI).comments;

			//Assert
			comments.Count.ShouldBe(1);
		}

		[Fact]
		public void GetAllSubmittedToLeadAnswersForURI_OnlyReturnsSubmittedToLeadAnswers()
		{
			//Arrange
			int locationId, answerId;
			var userId = Guid.NewGuid().ToString();
			var submissionId = AddSubmission(userId, _context);
			var organisationUserId = 1;
			var organisationId = 1;

			var questionTypeId = 99;
			locationId = AddLocation("consultations://./consultation/1/document/2/chapter/guidance", _context, "001.002.001.000");
			var questionId = AddQuestion(locationId, questionTypeId, "Question 1", _context);

			answerId = AddAnswer(questionId, userId, "This is a submitted answer", (int)StatusName.Submitted, _context);
			AddSubmissionAnswers(submissionId, answerId, _context);

			answerId = AddAnswer(questionId, null, "This is a draft answer", (int)StatusName.Draft, _context, organisationUserId, null, organisationId);
			AddSubmissionAnswers(submissionId, answerId, _context);

			answerId = AddAnswer(questionId, userId, "This is an answer submitted to an organisational lead", (int)StatusName.SubmittedToLead, _context, organisationUserId, null, organisationId);
			AddSubmissionAnswers(submissionId, answerId, _context);

			const string sourceURI = "consultations://./consultation/1";
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Rose Tyler", userId: Guid.NewGuid().ToString(), organisationIdUserIsLeadOf: organisationId);
			var context = new ConsultationsContext(_options, userService, _fakeEncryption);

			//Act
			var answers = context.GetCommentsAndAnswersSubmittedToALeadForURI(sourceURI).answers;

			//Assert
			answers.Count.ShouldBe(1);
		}

		[Fact]
		public void GetLeadsCommentsForURI_OnlyShowsCommentsSubmittedToCurrentLeadsOrg()
		{
			//Arrange
			int locationId, commentId;
			string userId = null;
			var submissionId = AddSubmission(userId, _context);
			var organisationUserId = 1;
			var organisationId = 1;

			locationId = AddLocation("consultations://./consultation/1", _context, "001.000.000.000");
			commentId = AddComment(locationId, "Comment submitted to lead", null, (int)StatusName.SubmittedToLead, _context, organisationUserId, null, organisationId);
			submissionId = AddSubmission(userId, _context);
			AddSubmissionComments(submissionId, commentId, _context);

			locationId = AddLocation("consultations://./consultation/1/document/1/chapter/chapter-slug", _context, "001.002.000.000");
			commentId = AddComment(locationId, "Comment submitted to lead of a different organisation", Guid.NewGuid().ToString(), (int)StatusName.SubmittedToLead, _context, 2, null, 2);
			AddSubmissionComments(submissionId, commentId, _context);

			const string sourceURI = "consultations://./consultation/1";
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Rose Tyler", userId: Guid.NewGuid().ToString(), organisationIdUserIsLeadOf: organisationId);
			var context = new ConsultationsContext(_options, userService, _fakeEncryption);

			//Act
			var comments = context.GetCommentsAndAnswersSubmittedToALeadForURI(sourceURI).comments;

			//Assert
			comments.Count.ShouldBe(1);
		}

		[Fact]
		public void GetSubmittedToLeadDataForConsulationForOrganisationLead()
		{
			// Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			string userId = null;
			var organisationId = 1;
			var otherOrganisationId = 2;

			var organisationUserId = 1;
			var anotherOrganisationUserId = 2;
			var organisationUserIdFromDifferentOrg = 3;

			int commentId, questionId, answerId;
			var questionTypeId = 99;
			var locationId = AddLocation("consultations://./consultation/1/document/1", _context, "001.001.000.000");
			var submissionId = AddSubmission(userId, _context);

			//Add a comment
			commentId = AddComment(locationId, "Submitted comment from org user 1", userId, (int)StatusName.SubmittedToLead, _context, organisationUserId, organisationId: organisationId);
			AddSubmissionComments(submissionId, commentId, _context);

			//Add another comment from a different user from the same org
			commentId = AddComment(locationId, "Submitted comment from org user 2", userId, (int)StatusName.SubmittedToLead, _context, anotherOrganisationUserId, organisationId: organisationId);
			AddSubmissionComments(submissionId, commentId, _context);

			//Add a comment from a user from a different org
			commentId = AddComment(locationId, "Submitted comment from a org user to a different org", userId, (int)StatusName.SubmittedToLead, _context, organisationUserIdFromDifferentOrg, organisationId: otherOrganisationId);
			AddSubmissionComments(submissionId, commentId, _context);

			//Add a question with an answer
			questionId = AddQuestion(locationId, questionTypeId, "Question answered by org user 1 ", _context);
			answerId = AddAnswer(questionId, userId, "Org user 1s answer", (int)StatusName.SubmittedToLead, _context, organisationUserId, organisationId: organisationId);
			AddSubmissionAnswers(submissionId, answerId, _context);

			//Add a question with an answer from a user from a different org
			questionId = AddQuestion(locationId, questionTypeId, "Question answered by user from other org ", _context);
			answerId = AddAnswer(questionId, userId, "Other org users answer", (int)StatusName.SubmittedToLead, _context, organisationUserIdFromDifferentOrg, organisationId: otherOrganisationId);
			AddSubmissionAnswers(submissionId, answerId, _context);

			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Rose Tyler", userId: Guid.NewGuid().ToString(), organisationIdUserIsLeadOf: 1);
			var context = new ConsultationsContext(_options, userService, _fakeEncryption);
			var consultationService = new ConsultationService(context, new FakeFeedService(), new FakeLogger<ConsultationService>(), userService);
			var export = new ExportService(context, userService, consultationService, _feedService);

			//Act
			var resultTuple = export.GetDataSubmittedToLeadForConsultation(1);

			//Assert
			resultTuple.comment.Count().ShouldBe(2);
			resultTuple.answer.Count().ShouldBe(1);
		}

		[Fact]
		public async void CreateSpreadsheetForOrganisationLeadWithResponsesTheyRecieved()
		{
			//Arrange
			AppSettings.ConsultationListConfig = TestAppSettings.GetConsultationListConfig();
			const int organisationUserId = 1;
			const int secondOrganisationUserId = 2;
			const string userId = "1";
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Sarah Jane Smith", userId: userId, organisationIdUserIsLeadOf: 1, emailAddress: "sarah@smith.com");

			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var locationId = AddLocation(sourceURI, _context, "001.001.000.000");
			var location = new Location(sourceURI, null, null, null, null, null, null, null, null, null, null);
			var commentText = "A comment";
			var comments = new List<Models.Comment>
			{
				new Models.Comment(locationId, null, commentText, null, location, 3, null, organisationUserId: organisationUserId),
				new Models.Comment(locationId, null, commentText, null, location, 3, null, organisationUserId: secondOrganisationUserId)
			};
			comments.First().SubmissionComment.Add(new SubmissionComment(1, 1));
			comments.First().SubmissionComment.First().Submission = new Submission(null, DateTime.Now, false, "organisation", false, null, null);
			comments[1].SubmissionComment.Add(new SubmissionComment(1, 1));
			comments[1].SubmissionComment.First().Submission = new Submission(null, DateTime.Now, false, "organisation", false, null, null);
			var answers = new List<Models.Answer> { };
			var questions = new List<Models.Question> { };

			var fakeExportService = new FakeExportService(comments, answers, questions,
				new List<OrganisationUser>
				{
					new OrganisationUser(1, Guid.NewGuid(), DateTime.Now) { EmailAddress = "mickey@smith.com", OrganisationUserId = organisationUserId },
					new OrganisationUser(1, Guid.NewGuid(), DateTime.Now) { EmailAddress = "luke@smith.com", OrganisationUserId = secondOrganisationUserId }
				});
			var exportToExcel = new ExportToExcel(userService, fakeExportService, null);

			//Act
			var spreadsheet = await exportToExcel.ToSpreadsheet(comments, answers, questions);

			using (var reader = ExcelReaderFactory.CreateReader(spreadsheet))
			{
				var workSheet = reader.AsDataSet();
				var data = workSheet.Tables;

				//Assert
				var rows = data[Constants.Export.SheetName].Rows;
				rows.Count.ShouldBe(5);

				var headerRow = rows[2].ItemArray;
				headerRow.Length.ShouldBe(15);

				headerRow[1].ToString().ShouldBe("Email Address");

				var firstCommentRow = rows[3].ItemArray;
				firstCommentRow[7].ShouldBe(commentText);
				firstCommentRow[1].ShouldBe("luke@smith.com");

				var secondCommentRow = rows[4].ItemArray;
				secondCommentRow[7].ShouldBe(commentText);
				secondCommentRow[1].ShouldBe("mickey@smith.com");
			}
		}
		
	}
}
