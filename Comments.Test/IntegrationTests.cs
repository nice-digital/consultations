using comments;
using Comments.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Comments.Test
{
    public class IntegrationTests 
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        public IntegrationTests()
        {
            // Arrange
            var builder = new WebHostBuilder()
                .ConfigureServices(services => {
                    services.AddDbContext<ConsultationsContext>(options =>
                        options.UseInMemoryDatabase(databaseName: "test_db"));
                }).UseStartup(typeof(Startup));
            _server = new TestServer(builder);
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
