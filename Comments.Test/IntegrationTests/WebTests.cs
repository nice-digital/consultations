using Comments.Test.Infrastructure;
using System.Threading.Tasks;
using Xunit;

namespace Comments.Test.IntegrationTests
{
    public class WebTests : TestBase
    {
        [Fact]
        public async Task Get_Consultations_Homepage()
        {
            // Act
            var response = await _client.GetAsync("/consultations"); //this route is temporary.
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            responseString.ShouldMatchApproved();
        }

        [Fact]
        public async Task Get_React_Document_Page()
        {
            // Act
            var response = await _client.GetAsync("/consultations/1/1/introduction");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            responseString.ShouldMatchApproved();
        }
    }
}
