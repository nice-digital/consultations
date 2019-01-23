using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Comments.Configuration;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using NICE.Feeds;
using NICE.Feeds.Tests.Infrastructure;
using Shouldly;
using Xunit;
using TestBase = Comments.Test.Infrastructure.TestBase;

namespace Comments.Test.IntegrationTests.API.Export
{
	public class ExportBase : TestBase
	{
		protected readonly IFeedService FeedService;
		public ExportBase(TestUserType testUserType) : base(testUserType, Feed.ConsultationCommentsListMultiple)
		{
			var consultationList = new List<NICE.Feeds.Models.Indev.List.ConsultationList>
			{
				new NICE.Feeds.Models.Indev.List.ConsultationList { ConsultationId = 1, AllowedRole = testUserType.ToString() }
			};
			FeedService = new FakeFeedService(consultationList);
		}
	}

    public class ExportTests : ExportBase
	{
		public ExportTests() : base(TestUserType.CustomFictionalRole) { }

	    [Fact]
	    public async Task Create_Spreadsheet()
	    {
		    // Arrange
		    ResetDatabase();
		    _context.Database.EnsureCreated();
			
		    var userId = Guid.NewGuid();

		    var locationId = AddLocation("consultations://./consultation/1", _context, "001.000.000.000");
		    var commentId = AddComment(locationId, "Submitted comment", false, userId, (int)StatusName.Submitted, _context);
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

			var userId = Guid.NewGuid();

			var locationId = AddLocation("consultations://./consultation/1", _context, "001.000.000.000");
			var commentId = AddComment(locationId, "Submitted comment", false, userId, (int)StatusName.Submitted, _context);
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
