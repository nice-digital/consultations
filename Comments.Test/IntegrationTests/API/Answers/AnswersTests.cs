using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Comments.Test.Infrastructure;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using Comments.Models;
using Comments.Services;
using Microsoft.EntityFrameworkCore;

namespace Comments.Test.IntegrationTests.API.Answers
{
    public class AnswersTests : TestBase
    {
        [Fact]
        public async Task Create_Answer()
        {
            // Arrange
            var answer = new ViewModels.Answer(0, "answer text", false, DateTime.Now, Guid.Empty);  //TODO: Not sure about AnswerID
            var content = new StringContent(JsonConvert.SerializeObject(answer), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/consultations/api/answer", content);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            var deserialisedAnswer = JsonConvert.DeserializeObject<ViewModels.Answer>(responseString);
            deserialisedAnswer.AnswerId.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task Get_Answer()
        {
            // Arrange
            ResetDatabase();
            const string sourceURI = "/consultations/1/1/introduction";
            var commentText = Guid.NewGuid().ToString();
            var questionText = Guid.NewGuid().ToString();
            var answerText = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid();
            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            //var context = new ConsultationsContext(_options, userService);
            //context.Database.EnsureCreated();

            AddCommentsAndQuestionsAndAnswers(sourceURI, commentText, questionText, answerText, userId, _context);

            //var answer = new ViewModels.Answer(0, "answer text", false, DateTime.Now, Guid.Empty);
            //var content = new StringContent(JsonConvert.SerializeObject(answer), Encoding.UTF8, "application/json");
            //var response = await _client.PostAsync("/consultations/api/answer", content);
            //response.EnsureSuccessStatusCode();

            //_context.Question
            //    .Include(question => question.Answer);

            //var question = _context.Question
            //    .Single(q => q.QuestionId == 1);

            //_context.Entry(question)
            //    .Collection(q => q.Answer)
            //    .Load();

            var question = _context.GetQuestion(1);
            var ans = _context.GetAnswer(1);

            // Act
            var response = await _client.GetAsync($"consultations/api/answer/1");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var deserialisedAnswer = JsonConvert.DeserializeObject<ViewModels.Answer>(responseString);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            deserialisedAnswer.AnswerId.ShouldBeGreaterThan(0);

            //context.Database.CloseConnection();
        }
    }
}
