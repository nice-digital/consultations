using System.Net.Http;
using comments;
using Comments.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Comments.Test.Infrastructure
{
    public class IntegrationTestBase
    {
        protected readonly TestServer _server;
        protected readonly HttpClient _client;
        public IntegrationTestBase()
        {
            // Arrange
            var builder = new WebHostBuilder()
                .UseContentRoot("../../../../Comments")
                .ConfigureServices(services => {
                    services.AddDbContext<ConsultationsContext>(options =>
                        options.UseInMemoryDatabase(databaseName: "test_db"));
                })
                .UseEnvironment("Production")
                .UseStartup(typeof(Startup));
            _server = new TestServer(builder);
            _client = _server.CreateClient();
        }
    }
}
