using System.Collections.Generic;
using System.Linq;
using System.Net;
using Comments.Configuration;
using Comments.Test.Infrastructure;
using NICE.Feeds.Tests.Infrastructure;
using System.Threading.Tasks;
using Comments.Models;
using Comments.ViewModels;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using TestBase = Comments.Test.Infrastructure.TestBase;

namespace Comments.Test.IntegrationTests.API.ConsultationList
{
	public class ConsultationListTests : TestBase
    {
	    public ConsultationListTests() : base(TestUserType.Administrator, Feed.ConsultationCommentsListMultiple,
		    new List<SubmittedCommentsAndAnswerCount>
		    {
			    new SubmittedCommentsAndAnswerCount
			    {
				    SourceURI = "consultations://./consultation/1",
				    TotalCount = 1
			    }
			},
			submittedToLeadCommentAndAnswerCount: new List<SubmittedCommentsAndAnswerCount>
			{
				new SubmittedCommentsAndAnswerCount
				{
					SourceURI = "consultations://./consultation/1",
					OrganisationId = 1,
					TotalCount = 1
				}
			})
	    {
			AppSettings.ConsultationListConfig = TestAppSettings.GetConsultationListConfig();
		    AppSettings.Feed = TestAppSettings.GetFeedConfig();
		}

        [Fact]
        public async Task Get_Consultation_Feed_Returns_Populated_Feed_For_Administrators()
        {
			//Arrange

			// Act
			var response = await _client.GetAsync("/consultations/api/ConsultationList?Status=Open");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            responseString.ShouldMatchApproved();
        }
	}

	public class ConsultationListNotAuthTests : TestBase
	{
		public ConsultationListNotAuthTests() : base(TestUserType.NotAuthenticated, Feed.ConsultationCommentsListMultiple)
		{
			AppSettings.ConsultationListConfig = TestAppSettings.GetConsultationListConfig();
			AppSettings.Feed = TestAppSettings.GetFeedConfig();
		}

		[Fact]
		public async Task Get_Consultation_Feed_Returns_Unauthorized_for_unauthorised_users()
		{
			//Arrange in constructor

			// Act 
			var response = await _client.GetAsync("/consultations/api/ConsultationList?Status=Open");

			//Assert
			response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
		}
	}

	public class ConsultationListAuthTests : TestBase
	{
		public ConsultationListAuthTests() : base(TestUserType.Authenticated, Feed.ConsultationCommentsListMultiple, bypassAuthentication: false)
		{
			AppSettings.ConsultationListConfig = TestAppSettings.GetConsultationListConfig();
			AppSettings.Feed = TestAppSettings.GetFeedConfig();
		}

		[Fact]
		public async Task Get_Consultation_Feed_Returns_Populated_Feed_For_Authenticated_User()
		{
			//Arrange

			// Act
			var response = await _client.GetAsync("/consultations/api/ConsultationList?Status=Open");
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();

			//Assert
			responseString.ShouldMatchApproved();
		}
	}

	public class ConsultationListAuthAdminTests : TestBase
	{
		public ConsultationListAuthAdminTests() : base(TestUserType.Administrator, Feed.ConsultationCommentsListMultiple, bypassAuthentication: false, submittedCommentsAndAnswerCounts: new List<SubmittedCommentsAndAnswerCount>(){ new SubmittedCommentsAndAnswerCount(){AnswerCount = 1,CommentCount = 1,SourceURI = "source uri", TotalCount = 2}})
		{
			AppSettings.ConsultationListConfig = TestAppSettings.GetConsultationListConfig();
			AppSettings.Feed = TestAppSettings.GetFeedConfig();
		}

		[Fact]
		public async Task Get_Consultation_Feed_Returns_Populated_Feed_For_Admin_User()
		{
			//Arrange

			// Act
			var response = await _client.GetAsync("/consultations/api/ConsultationList?Status=Open");
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();
			var model = JsonConvert.DeserializeObject<ConsultationListViewModel>(responseString);

			//Assert
			model.OptionFilters.Single().Options.Count(option => option.Id.Equals("Upcoming")).ShouldBe(1); //admins can see upcoming
			responseString.ShouldMatchApproved();
		}
	}

	public class ConsultationListAuthWithNoRolesTests : TestBase
	{
		public ConsultationListAuthWithNoRolesTests() : base(TestUserType.Authenticated, Feed.ConsultationCommentsListMultiple, bypassAuthentication: false, addRoleClaim: false)
		{
			AppSettings.ConsultationListConfig = TestAppSettings.GetConsultationListConfig();
			AppSettings.Feed = TestAppSettings.GetFeedConfig();
		}

		[Fact]
		public async Task Get_Consultation_Feed_Returns_Populated_Feed_For_Authenticated_User_With_No_Role()
		{
			//Arrange

			// Act
			var response = await _client.GetAsync("/consultations/api/ConsultationList?Status=Open");
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();
			var model = JsonConvert.DeserializeObject<ConsultationListViewModel>(responseString);

			//Assert
			model.OptionFilters.Single().Options.Count(option => option.Id.Equals("Upcoming")).ShouldBe(0); //regular authenticated users can't see upcoming
			responseString.ShouldMatchApproved();
		}
	}
}
