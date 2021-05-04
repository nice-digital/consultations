using Comments.Common;
using Comments.Models;
using Comments.Test.Infrastructure;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Xunit;
using Comment = Comments.ViewModels.Comment;

namespace Comments.Test.IntegrationTests.API.Comments
{
	public class EditCommentUsingOrganisationSessionCookieTests : TestBaseLight
	{
		private static readonly Guid _sessionId = Guid.Parse("11111111-1111-1111-1111-111111111111");

		private readonly TestServer _server;
		private readonly HttpClient _client;
		private readonly int consultationId = 1;
		private readonly Comment comment;

		public EditCommentUsingOrganisationSessionCookieTests()
		{
			const int organisationUserId = 1;
			var context = new ConsultationsContext(GetContextOptions(), FakeUserService.Get(isAuthenticated: false, testUserType: TestUserType.NotAuthenticated, organisationUserId: organisationUserId), new FakeEncryption());
			context.Database.EnsureDeleted();

			var sourceURI = $"consultations://./consultation/{consultationId}/document/1/chapter/introduction";

			var organisationAuthorisationId = TestBaseDBHelpers.AddOrganisationAuthorisationWithLocation(1, consultationId, context, null, "123412341234");
			TestBaseDBHelpers.AddOrganisationUser(context, organisationAuthorisationId, _sessionId, null);

			var locationId = TestBaseDBHelpers.AddLocation(context, sourceURI);
			var commentId = TestBaseDBHelpers.AddComment(context, locationId, "comment text", createdByUserId: null, organisationUserId: organisationUserId);

			(_server, _client) = InitialiseServerAndClient(context);

			comment = new ViewModels.Comment(1, sourceURI, null, null, null, null, null, null, null, 0,
					DateTime.Now, Guid.Empty.ToString(), "comment text", 1, show: true, sectionHeader: null, sectionNumber: null)
				{ CommentId = commentId };

		}

		[Fact]
		public async Task Edit_Comment_with_valid_organisation_session_cookie_returns_correctly()
		{
			//Arrange (mostly in the constructor)
			var builder = _server.CreateRequest($"/consultations/api/Comment/{comment.CommentId}");
			builder.AddHeader(HeaderNames.Cookie, $"{Constants.SessionCookieName}{consultationId}={_sessionId}");
			builder.And(request =>
			{
				request.Content = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");
			});

			//Act
			var response = await builder.SendAsync(HttpMethod.Put.Method);
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.OK);
			var deserialisedComment = JsonConvert.DeserializeObject<ViewModels.Comment>(responseString);
			deserialisedComment.CommentId.ShouldBe(comment.CommentId);
		}

		[Fact]
		public async Task Edit_Comment_with_invalid_organisation_session_cookie_returns_401()
		{
			//Arrange (mostly in the constructor)
			var builder = _server.CreateRequest($"/consultations/api/Comment/{comment.CommentId}");
			builder.AddHeader(HeaderNames.Cookie, $"{Constants.SessionCookieName}{consultationId}={Guid.NewGuid()}");
			builder.And(request =>
			{
				request.Content = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");
			});

			//Act
			var response = await builder.SendAsync(HttpMethod.Put.Method);

			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task Edit_Comment_with_invalid_consultation_id_in_organisation_session_cookie_returns_401()
		{
			//Arrange (mostly in the constructor)
			var builder = _server.CreateRequest($"/consultations/api/Comment/{comment.CommentId}");
			builder.AddHeader(HeaderNames.Cookie, $"{Constants.SessionCookieName}{999}={_sessionId}");
			builder.And(request =>
			{
				request.Content = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");
			});

			//Act
			var response = await builder.SendAsync(HttpMethod.Put.Method);

			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
		}
	}
}
