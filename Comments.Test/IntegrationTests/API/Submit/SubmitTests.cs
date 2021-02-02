using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Comments.Models;
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

			var commentId = AddComment(locationId, commentText, userId, (int)StatusName.Draft, _context);
			var questionTypeId = 99;
			var questionId = AddQuestion(locationId, questionTypeId, "Question Label");
			var answerId = AddAnswer(questionId, userId, "Answer Label");

			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
			//var submitService = new SubmitService(_context, userService, _consultationService);
			var commentService = new CommentService(_context, userService, _consultationService, _fakeHttpContextAccessor);

			var commentsAndQuestions = commentService.GetCommentsAndQuestions(sourceURI, _urlHelper);
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

		[Fact]
		public async Task Submit_Comments_To_Organisation_Lead()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();

			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var consultationId = 1;
			var organisationId = 1;
			var authorisationSession = Guid.NewGuid();
			var commentText = "Comment text";
			var emailAddress = "me@test.com";

			var organisationAuthorisationId = TestBaseDBHelpers.AddOrganisationAuthorisationWithLocation(organisationId, consultationId, _context);
			var organisationUserId = TestBaseDBHelpers.AddOrganisationUser(_context, organisationAuthorisationId, authorisationSession, null);

			var userService = FakeUserService.Get(true, "Benjamin Button", null, TestUserType.NotAuthenticated, false, organisationUserId);
			var consultationContext = new ConsultationsContext(_options, userService, _fakeEncryption);
			var commentService = new CommentService(consultationContext, userService, _consultationService, _fakeHttpContextAccessor);

			var locationId = AddLocation(sourceURI, _context);
			var commentId = AddComment(locationId, commentText, null, (int)StatusName.Draft, _context, organisationUserId, null, organisationId);

			var commentsAndQuestions = commentService.GetCommentsAndQuestions(sourceURI, _urlHelper);

			var submissionToLead = new SubmissionToLead(commentsAndQuestions.Comments, new List<ViewModels.Answer>(), emailAddress);
			var content = new StringContent(JsonConvert.SerializeObject(submissionToLead), Encoding.UTF8, "application/json");

			//Act
			var response = await _client.PostAsync($"consultations/api/SubmitToLead", content);
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();
			var comment = commentService.GetComment(commentId);
			var deserialisedCommentsAndAnswers = JsonConvert.DeserializeObject<ViewModels.Submission>(responseString);

			//Assert
			deserialisedCommentsAndAnswers.Comments.First().Status.StatusId.ShouldBe((int)StatusName.Submitted);
			deserialisedCommentsAndAnswers.Answers.First().Status.StatusId.ShouldBe((int)StatusName.Submitted);
		}
	}
}
