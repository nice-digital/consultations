using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Comments.Common;
using Comments.Models;
using Comments.Test.Infrastructure;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Comments.Test.IntegrationTests.API.Answers
{
	public class EditAnswerUsingOrganisationSessionCookieTests : TestBaseLight
	{
		private static readonly Guid _sessionId = Guid.Parse("11111111-1111-1111-1111-111111111111");

		private readonly TestServer _server;
		
		private const int ConsultationId = 1;
		private const int StatusId = 1;

		public readonly int _questionId;
		private readonly int _existingAnswerId;
		private readonly ConsultationsContext context;

		public EditAnswerUsingOrganisationSessionCookieTests()
		{
			const int organisationUserId = 1;
			
			context = new ConsultationsContext(GetContextOptions(), FakeUserService.Get(isAuthenticated: false, testUserType: TestUserType.NotAuthenticated, organisationUserId: organisationUserId), new FakeEncryption());
			context.Database.EnsureDeleted();

			var sourceURI = $"consultations://./consultation/{ConsultationId}/document/1/chapter/introduction";

			var organisationAuthorisationId = TestBaseDBHelpers.AddOrganisationAuthorisationWithLocation(1, ConsultationId, context, null, "123412341234");
			TestBaseDBHelpers.AddOrganisationUser(context, organisationAuthorisationId, _sessionId, null, organisationUserId);

			var locationId = TestBaseDBHelpers.AddLocation(context, sourceURI);
			_questionId = TestBaseDBHelpers.AddQuestion(context, locationId);
			_existingAnswerId = TestBaseDBHelpers.AddAnswer(context, _questionId, statusId: StatusId, organisationUserId: organisationUserId);
			(_server, _) = InitialiseServerAndClient(context);
		}

		[Fact]
		public async Task Edit_Comment_with_valid_organisation_session_cookie_returns_correctly()
		{
			//Arrange (mostly in the constructor)
			var updatedAnswerText = Guid.NewGuid().ToString();
			var updatedAnswer = new ViewModels.Answer(_existingAnswerId, updatedAnswerText, false, DateTime.UtcNow, "Carl Spackler", _questionId, StatusId);

			var builder = _server.CreateRequest($"/consultations/api/Answer/{_existingAnswerId}");
			builder.AddHeader(HeaderNames.Cookie, $"{Constants.SessionCookieName}{ConsultationId}={_sessionId}");
			builder.And(request =>
			{
				request.Content = new StringContent(JsonConvert.SerializeObject(updatedAnswer), Encoding.UTF8, "application/json");
			});

			//Act
			var response = await builder.SendAsync(HttpMethod.Put.Method);
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.OK);
			var deserialisedAnswer = JsonConvert.DeserializeObject<ViewModels.Answer>(responseString);
			deserialisedAnswer.AnswerId.ShouldBe(_existingAnswerId);
			deserialisedAnswer.AnswerText.ShouldBe(updatedAnswerText);
			context.Answer.IgnoreQueryFilters().Single(answer => answer.AnswerId.Equals(_existingAnswerId)).AnswerText.ShouldBe(updatedAnswerText);
		}

		[Fact]
		public async Task Edit_Answer_with_invalid_organisation_session_cookie_returns_401()
		{
			//Arrange (mostly in the constructor)
			var updatedAnswer = new ViewModels.Answer(_existingAnswerId, "updated answer text", false, DateTime.UtcNow, "Carl Spackler", _questionId, StatusId);
			var builder = _server.CreateRequest($"/consultations/api/Answer/{_existingAnswerId}");
			builder.AddHeader(HeaderNames.Cookie, $"{Constants.SessionCookieName}{ConsultationId}={Guid.NewGuid()}");
			builder.And(request =>
			{
				request.Content = new StringContent(JsonConvert.SerializeObject(updatedAnswer), Encoding.UTF8, "application/json");
			});

			//Act
			var response = await builder.SendAsync(HttpMethod.Put.Method);

			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task Edit_Answer_with_invalid_consultation_id_in_organisation_session_cookie_returns_401()
		{
			//Arrange (mostly in the constructor)
			var updatedAnswer = new ViewModels.Answer(_existingAnswerId, "updated answer text", false, DateTime.UtcNow, "Carl Spackler", _questionId, StatusId);
			var builder = _server.CreateRequest($"/consultations/api/Answer/{_existingAnswerId}");
			builder.AddHeader(HeaderNames.Cookie, $"{Constants.SessionCookieName}{999}={_sessionId}");
			builder.And(request =>
			{
				request.Content = new StringContent(JsonConvert.SerializeObject(updatedAnswer), Encoding.UTF8, "application/json");
			});

			//Act
			var response = await builder.SendAsync(HttpMethod.Put.Method);

			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
		}
	}
}
