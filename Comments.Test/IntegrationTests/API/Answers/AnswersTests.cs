﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Comments.Models;
using Comments.Services;
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
            var answer = new ViewModels.Answer(0, "answer text", false, DateTime.Now, Guid.Empty, 1);
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
            var answerText = Guid.NewGuid().ToString();
            var userId = Guid.Empty; 
            
            var answerId = AddAnswer(1, userId, answerText);
            
            // Act
            var response = await _client.GetAsync($"consultations/api/answer/{answerId}");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var deserialisedAnswer = JsonConvert.DeserializeObject<ViewModels.Answer>(responseString);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            deserialisedAnswer.AnswerId.ShouldBeGreaterThan(0);
            deserialisedAnswer.AnswerText.ShouldBe(answerText);
        }

        [Fact]
        public async Task Edit_Answer()
        {
            //Arrange
            var userId = Guid.Empty;
            var answerText = Guid.NewGuid().ToString();

            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);

            var answerId =  AddAnswer(1, userId, answerText, _context);
            
            var answerService = new AnswerService(_context, userService);
            var viewModel = answerService.GetAnswer(answerId);

            var updatedAnswerText = Guid.NewGuid().ToString();
            viewModel.answer.AnswerText = updatedAnswerText;

            var content = new StringContent(JsonConvert.SerializeObject(viewModel.answer), Encoding.UTF8, "application/json");

            //Act
            var response = await _client.PutAsync($"consultations/api/answer/{answerId}", content);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var result = answerService.GetAnswer(answerId);

            //Assert
            responseString.ShouldBe("1");
            result.answer.AnswerText.ShouldBe(updatedAnswerText);

        }

        [Fact]
        public async Task Delete_Answer()
        {
            //Arrange
            var userId = Guid.Empty;
            var answerText = Guid.NewGuid().ToString();
            var answerId = AddAnswer(1, userId, answerText);

            var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
            var answerService = new AnswerService(new ConsultationsContext(_options, userService), userService);

            //Act
            var response = await _client.DeleteAsync($"consultations/api/answer/{answerId}");
            response.EnsureSuccessStatusCode();

            var result =  answerService.GetAnswer(answerId);
           
            //Assert
            result.validate.NotFound.ShouldBeTrue();
        }
    }
}
