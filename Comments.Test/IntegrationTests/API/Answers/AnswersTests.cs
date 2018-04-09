using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Comments.Test.Infrastructure;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Comments.Test.IntegrationTests.API.Answers
{
    public class AnswersTests : TestBase
    {
        [Fact]
        public async Task Create_Answer()
        {
            // Arrange
            var answer = new ViewModels.Answer(0, "answer text", false, DateTime.Now, Guid.Empty);
            var content = new StringContent(JsonConvert.SerializeObject(answer), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/consultations/api/answer", content);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Get_Answer()
        {
            // Arrange
            ResetDatabase();
            var answerId = AddAnswer(1, Guid.NewGuid(), Guid.NewGuid().ToString());

            // Act
            var response = await _client.GetAsync("consultations/api/answer/1");
        }
    }
}
