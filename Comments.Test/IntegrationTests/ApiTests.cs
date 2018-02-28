using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Newtonsoft.Json;
using Shouldly;
using Shouldly.ShouldlyExtensionMethods;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Comments.Test.IntegrationTests
{
    public class ApiTests : TestBase
    {
        [Fact]
        public async Task GetDocumentReturnsEmptyFeed()
        {
            //Arrange (in the base constructor for this one.)

            // Act
            var response = await _client.GetAsync("/api/Document?consultationId=1&documentId=1");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            responseString.ShouldMatchApproved();
        }

        [Fact]
        public async Task GetDocumentReturnsPopulatedFeed()
        {
            // Arrange
            //ResetDatabase();
            var consultationId = 1;
            var documentId = 1;
            var commentText = Guid.NewGuid().ToString();
            var questionText = Guid.NewGuid().ToString();
            var answerText = Guid.NewGuid().ToString();

            AddCommentsAndQuestionsAndAnswers(consultationId, documentId, commentText, questionText, answerText);
            
            // Act
            var response = await _client.GetAsync($"/api/Document?consultationId={consultationId}&documentId={documentId}");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            responseString.ShouldMatchApproved();
            var deserialisedResponse = JsonConvert.DeserializeObject<DocumentViewModel>(responseString);
            deserialisedResponse.Comments.Single().CommentText.ShouldBe(commentText);
            deserialisedResponse.Questions.Single().QuestionText.ShouldBe(questionText);
            deserialisedResponse.Questions.Single().Answers.Single().AnswerText.ShouldBe(answerText);
        }
    }
}
