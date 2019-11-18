using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Comments.Services;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Comments.Test.IntegrationTests.API.Submit
{
    public class SubmitTests: TestBase
    {
	    public SubmitTests() : base(useRealSubmitService:true) {}

	    [Fact]
	    public async Task Submit_Comment_And_Answers()
	    {
		    //Arrange
		    ResetDatabase();
		    _context.Database.EnsureCreated();

			const string sourceURI = "consultations://./consultation/1/document/2/chapter/introduction";
		    var commentText = Guid.NewGuid().ToString();
		    var userId = Guid.Empty.ToString();
		    var locationId = AddLocation(sourceURI, _context);

			var commentId = AddComment(locationId, commentText, false, userId, (int)StatusName.Draft, _context);
			var questionTypeId = 99;
		    var questionId = AddQuestion(locationId, questionTypeId, "Question Label");
		    var answerId = AddAnswer(questionId, userId, "Answer Label");

			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			//var submitService = new SubmitService(_context, userService, _consultationService);
			var commentService = new CommentService(_context, userService, _consultationService, _linkGenerator, _fakeHttpContextAccessor);

		    var commentsAndQuestions = commentService.GetCommentsAndQuestions(sourceURI);
		    var viewModel = new ViewModels.Submission(commentsAndQuestions.Comments, commentsAndQuestions.Questions.First().Answers);
		
			var content = new StringContent(JsonConvert.SerializeObject(viewModel), Encoding.UTF8, "application/json");

		    //Act
		    var response = await _client.PostAsync($"consultations/api/Submit", content);  
		    response.EnsureSuccessStatusCode();
		    var responseString = await response.Content.ReadAsStringAsync();
			var comment = commentService.GetComment(commentId);
		    var deserialisedCommentsAndAnswers = JsonConvert.DeserializeObject<ViewModels.Submission>(responseString);

			//Assert
			comment.comment.StatusId.ShouldBe((int)StatusName.Submitted);
		    deserialisedCommentsAndAnswers.Comments.First().Status.Name.ShouldBe(StatusName.Submitted.ToString());
		    deserialisedCommentsAndAnswers.Comments.First().Status.StatusId.ShouldBe((int)StatusName.Submitted);
		    deserialisedCommentsAndAnswers.Answers.First().Status.Name.ShouldBe(StatusName.Submitted.ToString());
		    deserialisedCommentsAndAnswers.Answers.First().Status.StatusId.ShouldBe((int)StatusName.Submitted);
		}
	}
}
