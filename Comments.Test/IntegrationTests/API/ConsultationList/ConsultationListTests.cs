using Comments.Test.Infrastructure;
using System.Threading.Tasks;
using Comments.Services;
using NICE.Feeds.Tests.Infrastructure;
using Xunit;
using TestBase = Comments.Test.Infrastructure.TestBase;

namespace Comments.Test.IntegrationTests.API.ConsultationList
{
	public class ConsultationListTests : TestBase
    {
        public ConsultationListTests() : base(TestUserType.Administrator, Feed.ConsultationCommentsPublishedDetailMulitpleDoc) { }

        [Fact]
        public async Task Get_Consultation_Feed_Returns_Populated_Feed()
        {
			//Arrange
			//TODO: finish this
			//var consultationList = AddConsultationsToList();
			//var consultationListService = new ConsultationListService(_context, new FakeFeedService(consultationList), new FakeConsultationService());

			// Act
			var response = await _client.GetAsync("/consultations/api/ConsultationList?Status=Open");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            responseString.ShouldMatchApproved();
        }
	}
}
