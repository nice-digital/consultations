using System.Net;
using System.Security.Authentication;
using Comments.Test.Infrastructure;
using System.Threading.Tasks;
using Comments.Configuration;
using Comments.Services;
using NICE.Feeds.Tests.Infrastructure;
using Shouldly;
using Xunit;
using TestBase = Comments.Test.Infrastructure.TestBase;

namespace Comments.Test.IntegrationTests.API.ConsultationList
{
	public class ConsultationListTests : TestBase
    {
	    public ConsultationListTests() : base(TestUserType.Administrator, Feed.ConsultationCommentsListMultiple)
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

			// Act + Assert
			await Should.ThrowAsync<AuthenticationException>(() => _client.GetAsync("/consultations/api/ConsultationList?Status=Open"));
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
			//var ex = Assert.ThrowsAsync<AuthenticationException>(() => _client.GetAsync("/consultations/api/ConsultationList?Status=Open")).Result;

			//// Assert
			//ex.ShouldNotBeNull();
			
			Should.Throw<AuthenticationException>(() => _client.GetAsync("/consultations/api/ConsultationList?Status=Open"));
		}
	}
}
