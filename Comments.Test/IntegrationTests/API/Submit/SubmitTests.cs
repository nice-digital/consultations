using System;
using System.Collections.Generic;
using System.Linq;
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
	    public SubmitTests() : base(useFakeConsultationService:true) {}

	    [Fact]
	    public async Task Submit_Comment_And_Answers()
	    {
		    //Arrange
		    ResetDatabase();
		    _context.Database.EnsureCreated();

			const string sourceURI = "consultations://./consultation/1/document/2/chapter/introduction";
		    var commentText = Guid.NewGuid().ToString();
		    var userId = Guid.Empty;
		    var locationId = AddLocation(sourceURI, _context);

			var commentId = AddComment(locationId, commentText, false, userId, (int)StatusName.Draft, _context);
		    var questionTypeId = AddQuestionType("Question type description", false, true);
		    var questionId = AddQuestion(locationId, questionTypeId, "Question Text");
		    var answerId = AddAnswer(questionId, userId, "Answer Text");

			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
		    var authenticateService = new FakeAuthenticateService(authenticated: true);
			var commentService = new CommentService(_context, userService, authenticateService, _consultationService);

		    var commentsAndQuestions = commentService.GetCommentsAndQuestions(sourceURI, true);
		    var viewModel = new CommentsAndAnswers(commentsAndQuestions.Comments, commentsAndQuestions.Questions.First().Answers);
		
			var content = new StringContent(JsonConvert.SerializeObject(viewModel), Encoding.UTF8, "application/json");

		    //Act
		    var response = await _client.PostAsync($"consultations/api/Submit", content);  
		    response.EnsureSuccessStatusCode();
		    var responseString = await response.Content.ReadAsStringAsync();
			var comment = commentService.GetComment(commentId);
		    var deserialisedCommentsAndAnswers = JsonConvert.DeserializeObject<ViewModels.CommentsAndAnswers>(responseString);

			//Assert
			comment.comment.StatusId.ShouldBe((int)StatusName.Submitted);
		    deserialisedCommentsAndAnswers.Comments.First().Status.Name.ShouldBe(StatusName.Submitted.ToString());
		    deserialisedCommentsAndAnswers.Comments.First().Status.StatusId.ShouldBe((int)StatusName.Submitted);
		    deserialisedCommentsAndAnswers.Answers.First().Status.Name.ShouldBe(StatusName.Submitted.ToString());
		    deserialisedCommentsAndAnswers.Answers.First().Status.StatusId.ShouldBe((int)StatusName.Submitted);
		}
	}
}
