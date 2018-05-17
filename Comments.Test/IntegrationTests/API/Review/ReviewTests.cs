using System;
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

namespace Comments.Test.IntegrationTests.API.Review
{
    public class ReviewTests : TestBase
    {
        [Fact]
        public async Task Get_AllCommentsForConsultationForReview()
        {
            // Arrange
            ResetDatabase();

			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
	        var answerText = Guid.NewGuid().ToString();
	        var questionText = Guid.NewGuid().ToString();
	        var userId = Guid.Empty;

	        AddCommentsAndQuestionsAndAnswers(sourceURI, "My Comment", questionText, answerText, userId);
	        AddCommentsAndQuestionsAndAnswers(sourceURI, "My Second Comment", questionText, answerText, userId);
	        AddCommentsAndQuestionsAndAnswers(sourceURI, "Another users Comment", questionText, answerText, Guid.NewGuid());

			// Act
			var response = await _client.GetAsync("/consultations/api/Review/1");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var deserialisedObject = JsonConvert.DeserializeObject<ViewModels.CommentsAndQuestions>(responseString);

			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.OK);
			deserialisedObject.Comments.Count().ShouldBe(2);
		}
	}
}
