using Comments.Test.Infrastructure;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Comments.Test.IntegrationTests
{
    public class ApiTests : IntegrationTestBase
    {
        [Fact]
        public async Task GetDocumentReturnsFullFeed()
        {
            // Act
            var response = await _client.GetAsync("/api/Document?consultationId=1&documentId=1");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            responseString.ShouldMatchApproved();
        }
    }
}
