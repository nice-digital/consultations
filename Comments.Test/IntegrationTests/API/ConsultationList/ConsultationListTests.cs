using System.Collections.Generic;
using System.Net;
using Comments.Configuration;
using Comments.Test.Infrastructure;
using NICE.Feeds.Tests.Infrastructure;
using System.Threading.Tasks;
using Comments.Models;
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
		    })
	    {
			AppSettings.ConsultationListConfig = TestAppSettings.GetConsultationListConfig();
		    AppSettings.Feed = TestAppSettings.GetFeedConfig();
		}

        [Fact]
        public async Task Get_Consultation_Feed_Returns_Populated_Feed()
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
		public async Task Get_Consultation_Feed_Returns_Populated_Feed()
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
		public ConsultationListAuthTests() : base(TestUserType.Authenticated, Feed.ConsultationCommentsListMultiple)
		{
			AppSettings.ConsultationListConfig = TestAppSettings.GetConsultationListConfig();
			AppSettings.Feed = TestAppSettings.GetFeedConfig();
		}

		[Fact]
		public async Task Get_Consultation_Feed_Returns_Populated_Feed()
		{
			//Arrange

			// Act 
			var response = await _client.GetAsync("/consultations/api/ConsultationList?Status=Open");

			//Assert
			response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
		}
	}
}
