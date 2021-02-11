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
	public class SubmitTests : TestBase
	{
		public SubmitTests() : base(useRealSubmitService: true) {}

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

			var commentId = AddComment(locationId, commentText, userId, (int) StatusName.Draft, _context);
			var questionTypeId = 99;
			var questionId = AddQuestion(locationId, questionTypeId, "Question Label");
			var answerId = AddAnswer(questionId, userId, "Answer Label");

			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);

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
			comment.comment.StatusId.ShouldBe((int) StatusName.Submitted);
			deserialisedCommentsAndAnswers.Comments.First().Status.Name.ShouldBe(StatusName.Submitted.ToString());
			deserialisedCommentsAndAnswers.Comments.First().Status.StatusId.ShouldBe((int) StatusName.Submitted);
			deserialisedCommentsAndAnswers.Answers.First().Status.Name.ShouldBe(StatusName.Submitted.ToString());
			deserialisedCommentsAndAnswers.Answers.First().Status.StatusId.ShouldBe((int) StatusName.Submitted);
		}
	}

	public class SubmitToLeadTests : TestBaseLight
	{

		[Fact]
		public async Task Submit_Comments_To_Organisation_Lead()
		{
			//Arrange
			var organisationUserId = 1;
			var organisationId = 1;
			var fakeEncryption = new FakeEncryption();
			var context = new ConsultationsContext(GetContextOptions(),
				FakeUserService.Get(isAuthenticated: false, displayName: "Carl Spackler", userId: "Carl", testUserType: TestUserType.NotAuthenticated, organisationIdUserIsLeadOf: organisationId, organisationUserId: organisationUserId), fakeEncryption);
			context.Database.EnsureDeleted();

			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
			var consultationId = 1;

			var authorisationSession = Guid.NewGuid();
			var commentText = "Comment text";
			var emailAddress = "me@test.com";

			var organisationAuthorisationId = TestBaseDBHelpers.AddOrganisationAuthorisationWithLocation(organisationId, consultationId, context);
			TestBaseDBHelpers.AddOrganisationUser(context, organisationAuthorisationId, authorisationSession, null, organisationUserId: organisationUserId);
			TestBaseDBHelpers.AddStatus(context, nameof(StatusName.Submitted), (int)StatusName.Submitted);

			var userService = FakeUserService.Get(true, "Benjamin Button", null, TestUserType.NotAuthenticated, false, organisationUserId);
			
			var fakeHttpContextAccessor = FakeHttpContextAccessor.Get(isAuthenticated: false, testUserType: TestUserType.NotAuthenticated, organisationUserId: organisationUserId);
			var fakeConsultationService = new FakeConsultationService();
			var commentService = new CommentService(context, userService, fakeConsultationService, fakeHttpContextAccessor);

			var locationId = TestBaseDBHelpers.AddLocation(context, sourceURI);
			var questionId = TestBaseDBHelpers.AddQuestion(context, locationId);
			TestBaseDBHelpers.AddComment(context, locationId, commentText, null, (int)StatusName.Draft, organisationUserId, null, organisationId);
			TestBaseDBHelpers.AddAnswer(context, questionId, organisationUserId: organisationUserId);

			var commentsAndQuestions = commentService.GetCommentsAndQuestions(sourceURI, new FakeUrlHelper());

			var submissionToLead = new SubmissionToLead(commentsAndQuestions.Comments, commentsAndQuestions.Questions.First().Answers, emailAddress);
			var content = new StringContent(JsonConvert.SerializeObject(submissionToLead), Encoding.UTF8, "application/json");

			var (_server, _client) = InitialiseServerAndClient(context, userService, fakeConsultationService);

			//Act
			var response = await _client.PostAsync($"consultations/api/SubmitToLead", content);
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();

			var deserialisedCommentsAndAnswers = JsonConvert.DeserializeObject<ViewModels.SubmissionToLead>(responseString);

			//Assert
			deserialisedCommentsAndAnswers.Comments.First().Status.StatusId.ShouldBe((int)StatusName.Submitted);
			deserialisedCommentsAndAnswers.Answers.First().Status.StatusId.ShouldBe((int)StatusName.Submitted);
			deserialisedCommentsAndAnswers.EmailAddress.ShouldBe(emailAddress);
		}
	}
}
