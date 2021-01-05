using Comments.Common;
using Comments.Models;
using Comments.Test.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Comments.Test.IntegrationTests.API.Comments
{
	public class DeleteAnswerUsingOrganisationSessionCookieTests : TestBaseLight
	{
		private static readonly Guid _sessionId = Guid.Parse("11111111-1111-1111-1111-111111111111");
		private readonly int _consultationId = 1;
		const int StatusId = 1;

		[Fact]
		public async Task Delete_Comment_with_valid_organisation_session_cookie_deletes_correctly()
		{
			//Arrange
			const int organisationUserId = 1;

			var context = new ConsultationsContext(GetContextOptions(), FakeUserService.Get(isAuthenticated: false, testUserType: TestUserType.NotAuthenticated, organisationUserId: organisationUserId), new FakeEncryption());
			context.Database.EnsureDeleted();

			var sourceURI = $"consultations://./consultation/{_consultationId}/document/1/chapter/introduction";

			var organisationAuthorisationId = TestBaseDBHelpers.AddOrganisationAuthorisationWithLocation(1, _consultationId, context, null, "123412341234");
			TestBaseDBHelpers.AddOrganisationUser(context, organisationAuthorisationId, _sessionId, null, organisationUserId);

			var locationId = TestBaseDBHelpers.AddLocation(context, sourceURI);
			var _questionId = TestBaseDBHelpers.AddQuestion(context, locationId);
			var _existingAnswerId = TestBaseDBHelpers.AddAnswer(context, _questionId, statusId: StatusId, organisationUserId: organisationUserId);
			var (_server, _client) = InitialiseServerAndClient(context);

			var updatedAnswerText = Guid.NewGuid().ToString();
			var updatedAnswer = new ViewModels.Answer(_existingAnswerId, updatedAnswerText, false, DateTime.UtcNow, "Carl Spackler", _questionId, StatusId);

			var builder = _server.CreateRequest($"/consultations/api/Answer/{_existingAnswerId}");
			builder.AddHeader(HeaderNames.Cookie, $"{Constants.SessionCookieName}{_consultationId}={_sessionId}");
			builder.And(request =>
			{
				request.Content = new StringContent(JsonConvert.SerializeObject(updatedAnswer), Encoding.UTF8, "application/json");
			});

			//Act
			var response = await builder.SendAsync(HttpMethod.Delete.Method);
			response.EnsureSuccessStatusCode();
			
			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.OK);
			context.Answer.IgnoreQueryFilters().Count(answer => answer.AnswerId.Equals(_existingAnswerId)).ShouldBe(0);
		}

		[Fact]
		public async Task Delete_Comment_with_invalid_organisation_session_cookie_returns_401()
		{
			//Arrange
			const int organisationUserId = 1;

			var context = new ConsultationsContext(GetContextOptions(), FakeUserService.Get(isAuthenticated: false, testUserType: TestUserType.NotAuthenticated, organisationUserId: organisationUserId), new FakeEncryption());
			context.Database.EnsureDeleted();

			var sourceURI = $"consultations://./consultation/{_consultationId}/document/1/chapter/introduction";

			var organisationAuthorisationId = TestBaseDBHelpers.AddOrganisationAuthorisationWithLocation(1, _consultationId, context, null, "123412341234");
			TestBaseDBHelpers.AddOrganisationUser(context, organisationAuthorisationId, _sessionId, null, organisationUserId);

			var locationId = TestBaseDBHelpers.AddLocation(context, sourceURI);
			var _questionId = TestBaseDBHelpers.AddQuestion(context, locationId);
			var _existingAnswerId = TestBaseDBHelpers.AddAnswer(context, _questionId, statusId: StatusId, organisationUserId: organisationUserId);
			var (_server, _client) = InitialiseServerAndClient(context);

			var updatedAnswerText = Guid.NewGuid().ToString();
			var updatedAnswer = new ViewModels.Answer(_existingAnswerId, updatedAnswerText, false, DateTime.UtcNow, "Carl Spackler", _questionId, StatusId);

			var builder = _server.CreateRequest($"/consultations/api/Answer/{_existingAnswerId}");
			builder.AddHeader(HeaderNames.Cookie, $"{Constants.SessionCookieName}{_consultationId}={Guid.NewGuid()}");
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
			const int organisationUserId = 1;

			var context = new ConsultationsContext(GetContextOptions(), FakeUserService.Get(isAuthenticated: false, testUserType: TestUserType.NotAuthenticated, organisationUserId: organisationUserId), new FakeEncryption());
			context.Database.EnsureDeleted();

			var sourceURI = $"consultations://./consultation/{_consultationId}/document/1/chapter/introduction";

			var organisationAuthorisationId = TestBaseDBHelpers.AddOrganisationAuthorisationWithLocation(1, _consultationId, context, null, "123412341234");
			TestBaseDBHelpers.AddOrganisationUser(context, organisationAuthorisationId, _sessionId, null, organisationUserId);

			var locationId = TestBaseDBHelpers.AddLocation(context, sourceURI);
			var _questionId = TestBaseDBHelpers.AddQuestion(context, locationId);
			var _existingAnswerId = TestBaseDBHelpers.AddAnswer(context, _questionId, statusId: StatusId, organisationUserId: organisationUserId);
			var (_server, _client) = InitialiseServerAndClient(context);

			var updatedAnswerText = Guid.NewGuid().ToString();
			var updatedAnswer = new ViewModels.Answer(_existingAnswerId, updatedAnswerText, false, DateTime.UtcNow, "Carl Spackler", _questionId, StatusId);

			var builder = _server.CreateRequest($"/consultations/api/Answer/{_existingAnswerId}");
			builder.AddHeader(HeaderNames.Cookie, $"{Constants.SessionCookieName}{999}={_sessionId}");
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
