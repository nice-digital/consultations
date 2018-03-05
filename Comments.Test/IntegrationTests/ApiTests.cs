using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Newtonsoft.Json;
using Shouldly;
using Shouldly.ShouldlyExtensionMethods;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Comments.Test.IntegrationTests
{
    public class ApiTests : TestBase
    {
        [Fact]
        public async Task GetDocumentReturnsEmptyFeedIII()
        {
            //Arrange (in the base constructor for this one.)

            // Act
            var response = await _client.GetAsync("/api/Document?consultationId=1&documentId=1&chapterSlug=some-chapter");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            responseString.ShouldMatchApproved();
        }

        [Theory]
        [InlineData(1, 1, null)]
        [InlineData(1, 1, 1)]
        [InlineData(int.MaxValue, int.MaxValue, null)]
        [InlineData(int.MaxValue, int.MaxValue, 1)]
        public async Task CreateComment(int locationId, int consultationId, int? documentId)
        {
            //Arrange
            var comment = new ViewModels.Comment(locationId, consultationId, documentId, "chapter-slug", null, null, null, null, null, null, 0, DateTime.Now, Guid.Empty, "comment text");
            var content = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("api/Comment", content);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            var deserialisedComment = JsonConvert.DeserializeObject<ViewModels.Comment>(responseString);
            deserialisedComment.CommentId.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task Insert_Multiple_Comments_And_Read_Them()
        {
            //Arrange
            ResetDatabase();
            const int locationId = 1;
            const int consultationId = 1;
            const int documentId = 1;
            await CreateComment(locationId, consultationId, documentId);
            await CreateComment(locationId, consultationId, documentId); //duplicate comment. totally valid.
            await CreateComment(2, consultationId, documentId); //different location id, this should be in the result set
            await CreateComment(locationId, 2, documentId); //different consultation id, this shouldn't be in the result set
            await CreateComment(locationId, consultationId, 2); //different document id, this shouldn't be in the result set

            // Act
            var response = await _client.GetAsync($"/api/Document?consultationId={consultationId}&documentId={documentId}");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            var deserialisedResponse = JsonConvert.DeserializeObject<DocumentViewModel>(responseString);
            deserialisedResponse.Comments.Count().ShouldBe(3);
            responseString.ShouldMatchApproved();
        }

        [Fact]
        public async Task GetDocumentReturnsPopulatedFeed()
        {
            // Arrange
            ResetDatabase();
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
            //todo: get the below all working:

            //var deserialisedResponse = JsonConvert.DeserializeObject<DocumentViewModel>(responseString);
            //deserialisedResponse.Comments.Single().CommentText.ShouldBe(commentText);
            //deserialisedResponse.Questions.Single().QuestionText.ShouldBe(questionText);
            //deserialisedResponse.Questions.Single().Answers.Single().AnswerText.ShouldBe(answerText);
        }
    }
}
