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

		private const int ConsultationId = 1;
		private readonly TestServer _server;
		private readonly Comment _comment;

		public DeleteCommentUsingOrganisationSessionCookieTests()
		{
			const int organisationUserId = 1;
			var context = new ConsultationsContext(GetContextOptions(), FakeUserService.Get(isAuthenticated: false, testUserType: TestUserType.NotAuthenticated, organisationUserId: organisationUserId), new FakeEncryption());
			context.Database.EnsureDeleted();

			var sourceURI = $"consultations://./consultation/{ConsultationId}/document/1/chapter/introduction";

			var organisationAuthorisationId = TestBaseDBHelpers.AddOrganisationAuthorisationWithLocation(1, ConsultationId, context, null, "123412341234");
			TestBaseDBHelpers.AddOrganisationUser(context, organisationAuthorisationId, _sessionId, null, organisationUserId);

			var locationId = TestBaseDBHelpers.AddLocation(context, sourceURI);
			var commentId = TestBaseDBHelpers.AddComment(context, locationId, "comment text", createdByUserId: null, organisationUserId: organisationUserId);

			(_server, _) = InitialiseServerAndClient(context);

			_comment = new ViewModels.Comment(locationId, sourceURI, null, null, null, null, null, null, null, 0,
					DateTime.Now, Guid.Empty.ToString(), "comment text", 1, show: true, sectionHeader: null, sectionNumber: null)
				{ CommentId = commentId };

		}

		[Fact]
		public async Task Delete_Comment_with_valid_organisation_session_cookie_deletes_correctly()
		{
			//Arrange
			var builder = _server.CreateRequest($"/consultations/api/Comment/{_comment.CommentId}");
			builder.AddHeader(HeaderNames.Cookie, $"{Constants.SessionCookieName}{ConsultationId}={_sessionId}");
			builder.And(request =>
			{
				request.Content = new StringContent(JsonConvert.SerializeObject(_comment), Encoding.UTF8, "application/json");
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
			var builder = _server.CreateRequest($"/consultations/api/Comment/{_comment.CommentId}");
			builder.AddHeader(HeaderNames.Cookie, $"{Constants.SessionCookieName}{ConsultationId}={Guid.NewGuid()}");
			builder.And(request =>
			{
				request.Content = new StringContent(JsonConvert.SerializeObject(_comment), Encoding.UTF8, "application/json");
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
			var builder = _server.CreateRequest($"/consultations/api/Comment/{_comment.CommentId}");
			builder.AddHeader(HeaderNames.Cookie, $"{Constants.SessionCookieName}{999}={_sessionId}");
			builder.And(request =>
			{
				request.Content = new StringContent(JsonConvert.SerializeObject(_comment), Encoding.UTF8, "application/json");
			});

			//Act
			var response = await builder.SendAsync(HttpMethod.Delete.Method);

			// Assert
			response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
		}
	}
}
