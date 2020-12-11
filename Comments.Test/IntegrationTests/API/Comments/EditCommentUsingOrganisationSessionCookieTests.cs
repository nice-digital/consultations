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
using Xunit;

namespace Comments.Test.IntegrationTests.API.Comments
{
	public class EditCommentUsingOrganisationSessionCookieTests : TestBaseLight
	{
		private static readonly Guid _sessionId = Guid.Parse("11111111-1111-1111-1111-111111111111");

		[Fact]
		public async Task Edit_Comment_with_invalid_organisation_session_cookie_returns_correctly()
		{
			//Arrange
			var consultationId = 1;
			var context = new ConsultationsContext(GetContextOptions(), FakeUserService.Get(isAuthenticated: false, testUserType: TestUserType.NotAuthenticated, organisationUserId: 1), new FakeEncryption());

			var sourceURI = $"consultations://./consultation/{consultationId}/document/1/chapter/introduction";

			var organisationAuthorisationId = TestBaseDBHelpers.AddOrganisationAuthorisationWithLocation(1, consultationId, context, null, "123412341234");
			var organisationUserId = TestBaseDBHelpers.AddOrganisationUser(context, organisationAuthorisationId, _sessionId, null);

			var locationId = TestBaseDBHelpers.AddLocation(context, sourceURI);
			var commentId = TestBaseDBHelpers.AddComment(context, locationId, "comment text", createdByUserId: null, organisationUserId: organisationUserId);

			var (server, client) = InitialiseServerAndClient(context);

			var comment = new ViewModels.Comment(1, sourceURI, null, null, null, null, null, null, null, 0,
					DateTime.Now, Guid.Empty.ToString(), "comment text", 1, show: true, section: null)
				{CommentId = commentId};

			var builder = server.CreateRequest($"/consultations/api/Comment/{commentId}");

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
			deserialisedComment.CommentId.ShouldBe(commentId);
		}
	}
}
