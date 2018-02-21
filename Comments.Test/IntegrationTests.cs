using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using comments;
using Comments.Test.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Shouldly;
using Xunit;

namespace Comments.Test
{
    public class IntegrationTests : DatabaseSetup
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        public IntegrationTests()
        {
            // Arrange
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<TestStartup>()); //todo: use proper startup with some mocking.
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task GetDocumentReturnsFullFeed()
        {
            // Act
            var response = await _client.GetAsync("/api/Document?consultationId=00000000-0000-0000-0000-000000000000&documentId=00000000-0000-0000-0000-000000000000");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            responseString.ShouldBe("{\"title\":\"todo: title (and a bunch of other data) comes from the indev consultation feed\",\"locations\":[]}");
        }
    }
}
