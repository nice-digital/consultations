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
	public class DeleteCommentUsingOrganisationSessionCookieTests : TestBaseLight
	{
		private static readonly Guid _sessionId = Guid.Parse("11111111-1111-1111-1111-111111111111");

		//private readonly TestServer _server;
		//private readonly HttpClient _client;
		private readonly int consultationId = 1;
		//private readonly Comment comment;

		public DeleteCommentUsingOrganisationSessionCookieTests()
		{
			

		}

		[Fact]
		public async Task Delete_Comment_with_valid_organisation_session_cookie_deletes_correctly()
		{
			//Arrange
			var context = new ConsultationsContext(GetContextOptions(), FakeUserService.Get(isAuthenticated: false, testUserType: TestUserType.NotAuthenticated, organisationUserId: 1), new FakeEncryption());
			context.Database.EnsureDeleted();

			var sourceURI = $"consultations://./consultation/{consultationId}/document/1/chapter/introduction";

			var organisationAuthorisationId = TestBaseDBHelpers.AddOrganisationAuthorisationWithLocation(1, consultationId, context, null, "123412341234");
			var organisationUserId = TestBaseDBHelpers.AddOrganisationUser(context, organisationAuthorisationId, _sessionId, null);

			var locationId = TestBaseDBHelpers.AddLocation(context, sourceURI);
			var commentId = TestBaseDBHelpers.AddComment(context, locationId, "comment text", createdByUserId: null, organisationUserId: organisationUserId);

			var (_server, _client) = InitialiseServerAndClient(context);

			var comment = new ViewModels.Comment(locationId, sourceURI, null, null, null, null, null, null, null, 0,
					DateTime.Now, Guid.Empty.ToString(), "comment text", 1, show: true, section: null)
				{ CommentId = commentId };

			var builder = _server.CreateRequest($"/consultations/api/Comment/{comment.CommentId}");
			builder.AddHeader(HeaderNames.Cookie, $"{Constants.SessionCookieName}{consultationId}={_sessionId}");
			builder.And(request =>
			{
				request.Content = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");
			});

			//Act
			var response = await builder.SendAsync(HttpMethod.Delete.Method);
			response.EnsureSuccessStatusCode();
			
			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.OK);
		}

		[Fact]
		public async Task Delete_Comment_with_invalid_organisation_session_cookie_returns_401()
		{
			//Arrange 
			var context = new ConsultationsContext(GetContextOptions(), FakeUserService.Get(isAuthenticated: false, testUserType: TestUserType.NotAuthenticated, organisationUserId: 1), new FakeEncryption());
			context.Database.EnsureDeleted();

			var sourceURI = $"consultations://./consultation/{consultationId}/document/1/chapter/introduction";

			var organisationAuthorisationId = TestBaseDBHelpers.AddOrganisationAuthorisationWithLocation(1, consultationId, context, null, "123412341234");
			var organisationUserId = TestBaseDBHelpers.AddOrganisationUser(context, organisationAuthorisationId, _sessionId, null);

			var locationId = TestBaseDBHelpers.AddLocation(context, sourceURI);
			var commentId = TestBaseDBHelpers.AddComment(context, locationId, "comment text", createdByUserId: null, organisationUserId: organisationUserId);

			var (_server, _client) = InitialiseServerAndClient(context);

			var comment = new ViewModels.Comment(locationId, sourceURI, null, null, null, null, null, null, null, 0,
					DateTime.Now, Guid.Empty.ToString(), "comment text", 1, show: true, section: null)
			{ CommentId = commentId };

			var builder = _server.CreateRequest($"/consultations/api/Comment/{comment.CommentId}");
			builder.AddHeader(HeaderNames.Cookie, $"{Constants.SessionCookieName}{consultationId}={Guid.NewGuid()}");
			builder.And(request =>
			{
				request.Content = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");
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
			var context = new ConsultationsContext(GetContextOptions(), FakeUserService.Get(isAuthenticated: false, testUserType: TestUserType.NotAuthenticated, organisationUserId: 1), new FakeEncryption());
			context.Database.EnsureDeleted();

			var sourceURI = $"consultations://./consultation/{consultationId}/document/1/chapter/introduction";

			var organisationAuthorisationId = TestBaseDBHelpers.AddOrganisationAuthorisationWithLocation(1, consultationId, context, null, "123412341234");
			var organisationUserId = TestBaseDBHelpers.AddOrganisationUser(context, organisationAuthorisationId, _sessionId, null);

			var locationId = TestBaseDBHelpers.AddLocation(context, sourceURI);
			var commentId = TestBaseDBHelpers.AddComment(context, locationId, "comment text", createdByUserId: null, organisationUserId: organisationUserId);

			var (_server, _client) = InitialiseServerAndClient(context);

			var comment = new ViewModels.Comment(locationId, sourceURI, null, null, null, null, null, null, null, 0,
					DateTime.Now, Guid.Empty.ToString(), "comment text", 1, show: true, section: null)
			{ CommentId = commentId };

			var builder = _server.CreateRequest($"/consultations/api/Comment/{comment.CommentId}");
			builder.AddHeader(HeaderNames.Cookie, $"{Constants.SessionCookieName}{999}={_sessionId}");
			builder.And(request =>
			{
				request.Content = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");
			});

			//Act
			var response = await builder.SendAsync(HttpMethod.Delete.Method);

			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
		}
	}
}
