using Comments.Test.Infrastructure;
using System.Threading.Tasks;
using Xunit;

namespace Comments.Test.IntegrationTests
{
    public class WebTests : TestBase
    {
        //[Fact]
        //public async Task GetConsultationHomepage()
        //{
        //    // Act
        //    var response = await _client.GetAsync("/consultations/1/1");
        //    response.EnsureSuccessStatusCode();

        //    var responseString = await response.Content.ReadAsStringAsync();

        //    // Assert
        //    responseString.ShouldMatchApproved();
        //}

        [Fact]
        public async Task GetReactHomepage()
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
