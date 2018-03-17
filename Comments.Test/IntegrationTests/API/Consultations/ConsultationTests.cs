using Comments.Test.Infrastructure;
using NICE.Feeds.Tests.Infrastructure;
using System.Threading.Tasks;
using Xunit;
using TestBase = Comments.Test.Infrastructure.TestBase;

namespace Comments.Test.IntegrationTests.API.Consultations
{
    public class ConsultationsTests : TestBase
    {
        public ConsultationsTests() : base(Feed.ConsultationCommentsListMultiple) {}

        [Fact]
        public async Task Get_Consultations_Feed_Returns_Populated_Feed()
        {
            //Arrange (in base constructor)

            // Act
            var response = await _client.GetAsync("/consultations/api/Consultations");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            responseString.ShouldMatchApproved();
        }
    }
}