using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Comments.Services;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Comments.Test.IntegrationTests.API.Submit
{
    public class SubmitTests: TestBase
    {
	    [Fact]
	    public async Task Submit_Comment_And_Answers()
	    {
		    //Arrange
		    ResetDatabase();
		    var consultationId = 1;
		    const string sourceURI = "consultations://./consultation/1/document/2/chapter/introduction";
		    var commentText = Guid.NewGuid().ToString();
		    var userId = Guid.Empty;
		    var locationId = AddLocation(sourceURI, _context);

			var commentId = AddComment(locationId, commentText, false, userId, StatusName.Draft, _context);

			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
		    var authenticateService = new FakeAuthenticateService(authenticated: true);
			var commentService = new CommentService(_context, userService, authenticateService);

		    var viewModel = commentService.GetCommentsAndAnswers(sourceURI, true);

			var content = new StringContent(JsonConvert.SerializeObject(viewModel), Encoding.UTF8, "application/json");

		    //Act
		    var response = await _client.PostAsync($"consultations/api/Submit", content);  
		    response.EnsureSuccessStatusCode();
		    var responseString = await response.Content.ReadAsStringAsync();
			var result = commentService.GetComment(commentId);
		    var deserialisedComment = JsonConvert.DeserializeObject<ViewModels.CommentsAndAnswers>(responseString);

			//Assert
			result.comment.StatusId.ShouldBe(StatusName.Submitted);
		    
		}
	}
}
