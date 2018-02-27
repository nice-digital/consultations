using Comments.Test.Infrastructure;
using System.Threading.Tasks;
using Xunit;

namespace Comments.Test.IntegrationTests
{
    public class WebTests : IntegrationTestBase
    {
        [Fact]
        public async Task GetConsultationHomepage()
        {
            // Act
            var response = await _client.GetAsync("/consultations");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            responseString.ShouldMatchApproved();
        }
    }
}
