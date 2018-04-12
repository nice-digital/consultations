using System;
using System.Linq;
using System.Threading.Tasks;
using Comments.Test.Infrastructure;
using Xunit;
using Newtonsoft.Json;
using Shouldly;
using System.Net;
using System.Net.Http;
using System.Text;
using Comments.Models;
using Microsoft.EntityFrameworkCore;

namespace Comments.Test.IntegrationTests.API.Questions
{
    public class QuestionsTests : TestBase
    {
        [Fact]
        public async Task Get_Question()
        {
            //Arrange
            ResetDatabase();

            const string sourceURI = "/consultations/1/1/introduction";
            var commentText = Guid.NewGuid().ToString();
            var questionText = Guid.NewGuid().ToString();
            var answerText = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid();
            
            AddCommentsAndQuestionsAndAnswers(sourceURI, commentText, questionText, answerText, userId, _context);

            //Act
            var response = await _client.GetAsync($"consultations/api/question/1");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var deserialisedQuestion = JsonConvert.DeserializeObject<ViewModels.Question>(responseString);

            //Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            deserialisedQuestion.QuestionId.ShouldBeGreaterThan(0);
            deserialisedQuestion.Answers.Count().ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task Create_Question()
        {
            // Arrange
            var answer = new ViewModels.Question();
            var content = new StringContent(JsonConvert.SerializeObject(answer), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/consultations/api/question", content);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            var deserialisedAnswer = JsonConvert.DeserializeObject<ViewModels.Question>(responseString);
            deserialisedAnswer.QuestionId.ShouldBeGreaterThan(0);
        }
    }
}
