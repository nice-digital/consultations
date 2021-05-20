using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Comments.Common;
using Comments.Models;
using Comments.Test.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Comments.Test.IntegrationTests.API.Answers
{
	public class DeleteAnswerUsingOrganisationSessionCookieTests : TestBaseLight
	{
		private static readonly Guid SessionId = Guid.Parse("11111111-1111-1111-1111-111111111111");
		const int ConsultationId = 1;
		const int StatusId = 1;
		const int OrganisationUserId = 1;

		[Fact]
		public async Task Delete_Comment_with_valid_organisation_session_cookie_deletes_correctly()
		{
			//Arrange
			var context = new ConsultationsContext(GetContextOptions(), FakeUserService.Get(isAuthenticated: false, testUserType: TestUserType.NotAuthenticated, organisationUserId: OrganisationUserId), new FakeEncryption());
			context.Database.EnsureDeleted();

			var sourceURI = $"consultations://./consultation/{ConsultationId}/document/1/chapter/introduction";

			var organisationAuthorisationId = TestBaseDBHelpers.AddOrganisationAuthorisationWithLocation(1, ConsultationId, context, null, "123412341234");
			TestBaseDBHelpers.AddOrganisationUser(context, organisationAuthorisationId, SessionId, null, OrganisationUserId);

			var locationId = TestBaseDBHelpers.AddLocation(context, sourceURI);
			var questionId = TestBaseDBHelpers.AddQuestion(context, locationId);
			var existingAnswerId = TestBaseDBHelpers.AddAnswer(context, questionId, statusId: StatusId, organisationUserId: OrganisationUserId);
			var (server, client) = InitialiseServerAndClient(context);

			var updatedAnswerText = Guid.NewGuid().ToString();
			var updatedAnswer = new ViewModels.Answer(existingAnswerId, updatedAnswerText, false, DateTime.UtcNow, "Carl Spackler", questionId, StatusId);

			var builder = server.CreateRequest($"/consultations/api/Answer/{existingAnswerId}");
			builder.AddHeader(HeaderNames.Cookie, $"{Constants.SessionCookieName}{ConsultationId}={SessionId}");
			builder.And(request =>
			{
				request.Content = new StringContent(JsonConvert.SerializeObject(updatedAnswer), Encoding.UTF8, "application/json");
			});

			//Act
			var response = await builder.SendAsync(HttpMethod.Delete.Method);
			response.EnsureSuccessStatusCode();
			
			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.OK);
			context.Answer.IgnoreQueryFilters().Count(answer => answer.AnswerId.Equals(existingAnswerId)).ShouldBe(0);
		}

		[Fact]
		public async Task Delete_Comment_with_invalid_organisation_session_cookie_returns_401()
		{
			//Arrange
			var context = new ConsultationsContext(GetContextOptions(), FakeUserService.Get(isAuthenticated: false, testUserType: TestUserType.NotAuthenticated, organisationUserId: OrganisationUserId), new FakeEncryption());
			context.Database.EnsureDeleted();

			var sourceURI = $"consultations://./consultation/{ConsultationId}/document/1/chapter/introduction";

			var organisationAuthorisationId = TestBaseDBHelpers.AddOrganisationAuthorisationWithLocation(1, ConsultationId, context, null, "123412341234");
			TestBaseDBHelpers.AddOrganisationUser(context, organisationAuthorisationId, SessionId, null, OrganisationUserId);

			var locationId = TestBaseDBHelpers.AddLocation(context, sourceURI);
			var questionId = TestBaseDBHelpers.AddQuestion(context, locationId);
			var existingAnswerId = TestBaseDBHelpers.AddAnswer(context, questionId, statusId: StatusId, organisationUserId: OrganisationUserId);
			var (server, client) = InitialiseServerAndClient(context);

			var updatedAnswerText = Guid.NewGuid().ToString();
			var updatedAnswer = new ViewModels.Answer(existingAnswerId, updatedAnswerText, false, DateTime.UtcNow, "Carl Spackler", questionId, StatusId);

			var builder = server.CreateRequest($"/consultations/api/Answer/{existingAnswerId}");
			builder.AddHeader(HeaderNames.Cookie, $"{Constants.SessionCookieName}{ConsultationId}={Guid.NewGuid()}");
			builder.And(request =>
			{
				request.Content = new StringContent(JsonConvert.SerializeObject(updatedAnswer), Encoding.UTF8, "application/json");
			});

			//Act
			var response = await builder.SendAsync(HttpMethod.Delete.Method);

			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task Delete_Comment_with_invalid_consultation_id_in_organisation_session_cookie_returns_401()
		{
			//Arrange
			var context = new ConsultationsContext(GetContextOptions(), FakeUserService.Get(isAuthenticated: false, testUserType: TestUserType.NotAuthenticated, organisationUserId: OrganisationUserId), new FakeEncryption());
			context.Database.EnsureDeleted();

			var sourceURI = $"consultations://./consultation/{ConsultationId}/document/1/chapter/introduction";

			var organisationAuthorisationId = TestBaseDBHelpers.AddOrganisationAuthorisationWithLocation(1, ConsultationId, context, null, "123412341234");
			TestBaseDBHelpers.AddOrganisationUser(context, organisationAuthorisationId, SessionId, null, OrganisationUserId);

			var locationId = TestBaseDBHelpers.AddLocation(context, sourceURI);
			var questionId = TestBaseDBHelpers.AddQuestion(context, locationId);
			var existingAnswerId = TestBaseDBHelpers.AddAnswer(context, questionId, statusId: StatusId, organisationUserId: OrganisationUserId);
			var (server, client) = InitialiseServerAndClient(context);

			var updatedAnswerText = Guid.NewGuid().ToString();
			var updatedAnswer = new ViewModels.Answer(existingAnswerId, updatedAnswerText, false, DateTime.UtcNow, "Carl Spackler", questionId, StatusId);

			var builder = server.CreateRequest($"/consultations/api/Answer/{existingAnswerId}");
			builder.AddHeader(HeaderNames.Cookie, $"{Constants.SessionCookieName}{999}={SessionId}");
			builder.And(request =>
			{
				request.Content = new StringContent(JsonConvert.SerializeObject(updatedAnswer), Encoding.UTF8, "application/json");
			});

			//Act
			var response = await builder.SendAsync(HttpMethod.Delete.Method);

			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
		}
	}
}
