using Comments.Common;
using Comments.Configuration;
using Comments.Models;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using ExcelDataReader;
using NICE.Feeds.Indev;
using NICE.Feeds.Tests.Infrastructure;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using TestBase = Comments.Test.Infrastructure.TestBase;

namespace Comments.Test.IntegrationTests.API.Export
{
	public class ExportBase : TestBase
	{
		protected readonly IIndevFeedService FeedService;
		public ExportBase(TestUserType testUserType) : base(testUserType, Feed.ConsultationCommentsListMultiple)
		{
			var consultationList = new List<NICE.Feeds.Indev.Models.List.ConsultationList>
			{
				new NICE.Feeds.Indev.Models.List.ConsultationList { ConsultationId = 1, AllowedRole = testUserType.ToString() }
			};
			FeedService = new FakeFeedService(consultationList);
		}
	}

	public class ExportTests : ExportBase
	{
		public ExportTests() : base(TestUserType.CustomFictionalRole)
		{
		}

		[Fact]
		public async Task Create_Spreadsheet()
		{
			// Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();

			var userId = Guid.NewGuid().ToString();

			var locationId = AddLocation("consultations://./consultation/1", _context, "001.000.000.000");
			var commentId = AddComment(locationId, "Submitted comment", userId, (int) StatusName.Submitted,
				_context);
			var submissionId = AddSubmission(userId, _context);
			AddSubmissionComments(submissionId, commentId, _context);

			const int consultationId = 1;

			// Act
			var response = await _client.GetAsync($"consultations/api/ExportExternal/{consultationId}");
			response.EnsureSuccessStatusCode();

			//Assert
			response.IsSuccessStatusCode.ShouldBeTrue();
		}
	}
	public class ExportTestsWithData : TestBase
	{
		public ExportTestsWithData(bool useRealSubmitService = false, TestUserType testUserType = TestUserType.Authenticated, bool useFakeConsultationService = false, IList<SubmittedCommentsAndAnswerCount> submittedCommentsAndAnswerCounts = null)
			: base(useRealSubmitService, testUserType, useFakeConsultationService, submittedCommentsAndAnswerCounts)
		{
			System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
		}

		[Fact]
		public async Task Create_Spreadsheet_Which_does_not_have_the_express_interest_checked()
		{
			// Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var commentText = Guid.NewGuid().ToString();
			const int consultationId = 1;

			var locationId = AddLocation($"consultations://./consultation/{consultationId}", _context, "001.000.000.000");
			var commentId = AddComment(locationId, commentText, _userId, (int)StatusName.Submitted);
			var submissionId = AddSubmission(_userId);
			AddSubmissionComments(submissionId, commentId);

			// Act
			var response = await _client.GetAsync($"consultations/api/ExportExternal/{consultationId}");
			response.EnsureSuccessStatusCode();

			var excelStream = response.Content.ReadAsStreamAsync().Result;

			using (var reader = ExcelReaderFactory.CreateReader(excelStream))
			{
				var workSheet = reader.AsDataSet();
				var data = workSheet.Tables;

				//Assert
				var rows = data[Constants.Export.SheetName].Rows;
				rows.Count.ShouldBe(4);

				var headerRow = rows[2].ItemArray;
				headerRow.Length.ShouldBe(15);
				headerRow.Any(header => header.ToString().Equals(Constants.Export.ExpressionOfInterestColumnDescription, StringComparison.OrdinalIgnoreCase)).ShouldBeFalse();

				var commentRow = rows[3].ItemArray;
				commentRow[7].ShouldBe(commentText);
			}
		}

		[Fact]
		public async Task Create_Spreadsheet_Which_does_have_the_express_interest_checked()
		{
			// Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var commentText = Guid.NewGuid().ToString();
			const int consultationId = 1;

			var locationId = AddLocation($"consultations://./consultation/{consultationId}", _context, "001.000.000.000");
			var commentId = AddComment(locationId, commentText, _userId, (int)StatusName.Submitted);
			var submissionId = AddSubmission(_userId, null, organisationExpressionOfInterest: true);
			AddSubmissionComments(submissionId, commentId);

			// Act
			var response = await _client.GetAsync($"consultations/api/ExportExternal/{consultationId}");
			response.EnsureSuccessStatusCode();

			var excelStream = response.Content.ReadAsStreamAsync().Result;

			using (var reader = ExcelReaderFactory.CreateReader(excelStream))
			{
				var workSheet = reader.AsDataSet();
				var data = workSheet.Tables;

				//Assert
				var rows = data[Constants.Export.SheetName].Rows;
				rows.Count.ShouldBe(4);

				var headerRow = rows[2].ItemArray;
				headerRow.Length.ShouldBe(16);

				headerRow[15].ToString().ShouldBe(Constants.Export.ExpressionOfInterestColumnDescription);

				var commentRow = rows[3].ItemArray;
				commentRow[7].ShouldBe(commentText);
				commentRow[15].ShouldBe(Constants.Export.Yes);
			}
		}

		[Fact]
		public async Task Create_Spreadsheet_Which_has_the_express_interest_set_to_no()
		{
			// Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var commentText = Guid.NewGuid().ToString();
			const int consultationId = 1;

			var locationId = AddLocation($"consultations://./consultation/{consultationId}", _context, "001.000.000.000");
			var commentId = AddComment(locationId, commentText, _userId, (int)StatusName.Submitted);
			var submissionId = AddSubmission(_userId, null, organisationExpressionOfInterest: false);
			AddSubmissionComments(submissionId, commentId);

			// Act
			var response = await _client.GetAsync($"consultations/api/ExportExternal/{consultationId}");
			response.EnsureSuccessStatusCode();

			var excelStream = response.Content.ReadAsStreamAsync().Result;

			using (var reader = ExcelReaderFactory.CreateReader(excelStream))
			{
				var workSheet = reader.AsDataSet();
				var data = workSheet.Tables;

				//Assert
				var rows = data[Constants.Export.SheetName].Rows;
				rows.Count.ShouldBe(4);

				var headerRow = rows[2].ItemArray;
				headerRow.Length.ShouldBe(16);

				headerRow[15].ToString().ShouldBe(Constants.Export.ExpressionOfInterestColumnDescription);

				var commentRow = rows[3].ItemArray;
				commentRow[7].ShouldBe(commentText);
				commentRow[15].ShouldBe(Constants.Export.No);
			}
		}
	}

	public class ExportToExcelTestsForNonAdminUser : ExportBase
	{
		public ExportToExcelTestsForNonAdminUser() : base(TestUserType.Authenticated)
		{
			AppSettings.Feed = TestAppSettings.GetFeedConfig();
		}

		[Fact]
		public async Task None_Admin_Cannot_Create_Spreadsheet()
		{
			// Arrange
			ResetDatabase();
			AppSettings.ConsultationListConfig = TestAppSettings.GetConsultationListConfig();
			_context.Database.EnsureCreated();

			var userId = Guid.NewGuid().ToString();

			var locationId = AddLocation("consultations://./consultation/1", _context, "001.000.000.000");
			var commentId = AddComment(locationId, "Submitted comment", userId, (int)StatusName.Submitted, _context);
			var submissionId = AddSubmission(userId, _context);
			AddSubmissionComments(submissionId, commentId, _context);

			const int consultationId = 1;

			// Act
			var response = await _client.GetAsync($"consultations/api/Export/{consultationId}");

			//Assert
			response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
		}
	}
}
