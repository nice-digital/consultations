using Comments.Common;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Comments.Models;
using Xunit;
using Answer = Comments.ViewModels.Answer;

namespace Comments.Test.IntegrationTests.API.Comments
{
	public class CreateAnswerUsingOrganisationSessionCookieTests : TestBaseLight
	{
		private static readonly Guid _sessionId = Guid.Parse("11111111-1111-1111-1111-111111111111");

		//public CreateAnswerUsingOrganisationSessionCookieTests() : base(enableOrganisationalCommentingFeature: true, testUserType: TestUserType.NotAuthenticated,
		//	validSessions: new Dictionary<int, Guid>{{ _consultationId, _sessionId }}, useRealUserService: true, useRealHttpContextAccessor: true)
		//{
		//}

		[Fact]
		public async Task Create_Answer_With_Invalid_Organisation_Session_Cookie_Returns_401()
		{
			//Arrange
			//var locationId = TestBaseDBHelpers.AddLocation(context, sourceURI);
			//var questionId = TestBaseDBHelpers.AddQuestion(context, locationId);
			const int consultationId = 1;
			var fakeUserService = FakeUserService.Get(isAuthenticated: false, userId: null, displayName: null, testUserType: TestUserType.NotAuthenticated, organisationUserId: null, organisationIdUserIsLeadOf: null);

			
			var context = new ConsultationsContext(GetContextOptions(), fakeUserService, new FakeEncryption());
			var answer = new ViewModels.Answer(0, "answer text", false, DateTime.UtcNow, "Carl Spackler", 1, 1);
			var (_server, _client) = InitialiseServerAndClient(context, fakeUserService);
			var builder = _server.CreateRequest("/consultations/api/Answer");

			builder.AddHeader(HeaderNames.Cookie, $"{Constants.SessionCookieName}{consultationId}={Guid.NewGuid()}");

			builder.And(request =>
			{
				request.Content = new StringContent(JsonConvert.SerializeObject(answer), Encoding.UTF8, "application/json");
			});

			// Act
			var response = await builder.PostAsync();
			
			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task Create_Answer_With_Valid_Organisation_Session_Cookie_For_Incorrect_Consultation_Returns_403()
		{
			//Arrange
			const int consultationId = 1;
			const int organisationUserId = 1;
			const int organisationId = 1;
			var fakeUserService = FakeUserService.Get(isAuthenticated: false, userId: null, displayName: null, testUserType: TestUserType.NotAuthenticated, organisationUserId: organisationUserId, organisationIdUserIsLeadOf: null);
			var context = new ConsultationsContext(GetContextOptions(), fakeUserService, new FakeEncryption());


			var organisationAuthorisationId = TestBaseDBHelpers.AddOrganisationAuthorisationWithLocation(organisationId, consultationId, context);
			TestBaseDBHelpers.AddOrganisationUser(context, organisationAuthorisationId, _sessionId, null, organisationUserId);

			var differentNotValidConsultationId = 99;
			var locationId = TestBaseDBHelpers.AddLocation(context, $"consultations://./consultation/{differentNotValidConsultationId}/document/1");
			var questionId = TestBaseDBHelpers.AddQuestion(context, locationId);

			var (_server, _client) = InitialiseServerAndClient(context);

			var answer = new ViewModels.Answer(0, "answer text", false, DateTime.UtcNow, "Carl Spackler", questionId, 1);

			var builder = _server.CreateRequest("/consultations/api/Answer");

			builder.AddHeader(HeaderNames.Cookie, $"{Constants.SessionCookieName}{consultationId}={_sessionId}");

			builder.And(request =>
			{
				request.Content = new StringContent(JsonConvert.SerializeObject(answer), Encoding.UTF8, "application/json");
			});

			// Act
			var response = await builder.PostAsync();

			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
		}

		[Fact]
		public async Task Create_Answer_With_Valid_Organisation_Session_Cookie_Returns_Correctly()
		{
			//Arrange
			const int consultationId = 1;
			const int organisationUserId = 1;
			const int organisationId = 1;
			const int statusId = 1;
			var fakeUserService = FakeUserService.Get(isAuthenticated: false, userId: null, displayName: null, testUserType: TestUserType.NotAuthenticated, organisationUserId: organisationUserId, organisationIdUserIsLeadOf: null);
			var context = new ConsultationsContext(GetContextOptions(), fakeUserService, new FakeEncryption());

			TestBaseDBHelpers.AddStatus(context, "Draft", statusId);
			var organisationAuthorisationId = TestBaseDBHelpers.AddOrganisationAuthorisationWithLocation(organisationId, consultationId, context);
			TestBaseDBHelpers.AddOrganisationUser(context, organisationAuthorisationId, _sessionId, null, organisationUserId);

			var locationId = TestBaseDBHelpers.AddLocation(context, $"consultations://./consultation/{consultationId}/document/1");
			var questionId = TestBaseDBHelpers.AddQuestion(context, locationId);

			var (_server, _client) = InitialiseServerAndClient(context);

			var answer = new ViewModels.Answer(0, "answer text", false, DateTime.UtcNow, "Carl Spackler", questionId, statusId);

			var builder = _server.CreateRequest("/consultations/api/Answer");

			builder.AddHeader(HeaderNames.Cookie, $"{Constants.SessionCookieName}{consultationId}={_sessionId}");

			builder.And(request =>
			{
				request.Content = new StringContent(JsonConvert.SerializeObject(answer), Encoding.UTF8, "application/json");
			});

			// Act
			var response = await builder.PostAsync();
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.Created);
			var deserialisedAnswer = JsonConvert.DeserializeObject<Answer>(responseString);
			deserialisedAnswer.AnswerId.ShouldBeGreaterThan(0);
			var answerInDatabase = context.Answer.IgnoreQueryFilters().Single(dbAnswer => dbAnswer.AnswerId.Equals(deserialisedAnswer.AnswerId));

			answerInDatabase.CreatedByUserId.ShouldBeNull();
			answerInDatabase.OrganisationUserId.ShouldBe(organisationUserId);
			answerInDatabase.ParentAnswerId.ShouldBeNull();
			answerInDatabase.OrganisationId.Value.ShouldBe(organisationId);
		}
	}
}
