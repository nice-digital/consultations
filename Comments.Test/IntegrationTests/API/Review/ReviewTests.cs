using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Answer = Comments.ViewModels.Answer;
using Comment = Comments.ViewModels.Comment;

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

	    [Fact]
	    public async Task Submit_Comments()
	    {
			//Arrange
		    var comment = new ViewModels.Comment(1, "consultations://./consultation/1/document/1/chapter/introduction", null, null, null, null, null, null, 0, DateTime.Now, Guid.Empty, "comment text", 1);
		    var commentsAndAnswers = new CommentsAndAnswers(comments: new List<Comment>{comment}, answers: new List<Answer>());
		    var content = new StringContent(JsonConvert.SerializeObject(commentsAndAnswers), Encoding.UTF8, "application/json");

		    // Act
		    var response = await _client.PostAsync("/consultations/api/Review/1", content);
		    response.EnsureSuccessStatusCode();
		    var responseString = await response.Content.ReadAsStringAsync();

		    // Assert
		    response.StatusCode.ShouldBe(HttpStatusCode.OK);
	    }
	}
}
