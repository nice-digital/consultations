using Comments.Models;
using Comments.Test.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Comments.Common;
using Microsoft.Net.Http.Headers;
using Xunit;

namespace Comments.Test.IntegrationTests.API.Comments
{
	public class CreateCommentAsOrganisationalLead : TestBaseLight
	{
		private const int ConsultationId = 1;
		private static readonly Guid _sessionId = Guid.Parse("11111111-1111-1111-1111-111111111111");

		[Fact]
		public async Task Create_Comment_As_Lead_Sets_Comment_Fields_Correctly()
		{
			//Arrange
			const int organisationId = 1;
			var orgLeadUserId = Guid.NewGuid().ToString();
			
			var fakeUserService = FakeUserService.Get(isAuthenticated: true, userId: orgLeadUserId, displayName: "The Judge", testUserType: TestUserType.Authenticated, organisationUserId: null, organisationIdUserIsLeadOf: organisationId);

			using (var context = new ConsultationsContext(GetContextOptions(), fakeUserService, new FakeEncryption()))
			{
				context.Database.EnsureDeleted();
				var sourceURI = $"consultations://./consultation/{ConsultationId}/document/1";
				TestBaseDBHelpers.AddStatus(context, "Draft", 1);
				var (_server, _client) = InitialiseServerAndClient(context, fakeUserService);

				var comment = new ViewModels.Comment(1, sourceURI, null, null, null, null, null, null, null, 0,
					DateTime.Now, Guid.Empty.ToString(), "comment text", 1, show: true, section: null);

				var builder = _server.CreateRequest($"/consultations/api/Comment");
				builder.And(request =>
				{
					request.Content = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");
				});

				//Act
				var response = await builder.PostAsync();
				response.EnsureSuccessStatusCode();

				var responseString = await response.Content.ReadAsStringAsync();
				var deserialisedComment = JsonConvert.DeserializeObject<ViewModels.Comment>(responseString);

				var commentInDatabase = context.Comment.IgnoreQueryFilters().Single(dbComment => dbComment.CommentId.Equals(deserialisedComment.CommentId));

				// Assert
				response.StatusCode.ShouldBe(HttpStatusCode.Created);
				commentInDatabase.CreatedByUserId.ShouldBe(orgLeadUserId);
				commentInDatabase.OrganisationUserId.ShouldBeNull();
				commentInDatabase.ParentCommentId.ShouldBeNull();
				commentInDatabase.OrganisationId.ShouldBe(organisationId);
			}
		}

		[Fact]
		public async Task Create_Comment_As_Lead_And_With_Organisation_Cookie() //tests multiple authentication mechanisms. - the cookie should win.
		{
			//Arrange
			const int organisationId = 1;
			var orgLeadUserId = Guid.NewGuid().ToString();
			var organisationUserId = 1;

			var fakeUserService = FakeUserService.Get(isAuthenticated: true, userId: orgLeadUserId, displayName: "The Judge", testUserType: TestUserType.Authenticated, organisationUserId: organisationUserId, organisationIdUserIsLeadOf: organisationId);

			using (var context = new ConsultationsContext(GetContextOptions(), fakeUserService, new FakeEncryption()))
			{
				context.Database.EnsureDeleted();
				var sourceURI = $"consultations://./consultation/{ConsultationId}/document/1";
				TestBaseDBHelpers.AddStatus(context, "Draft", 1);

				var organisationAuthorisationId = TestBaseDBHelpers.AddOrganisationAuthorisationWithLocation(organisationId, ConsultationId, context, null, "123412341234");
				TestBaseDBHelpers.AddOrganisationUser(context, organisationAuthorisationId, _sessionId, null, organisationUserId);

				var (_server, _client) = InitialiseServerAndClient(context, fakeUserService);

				var comment = new ViewModels.Comment(1, sourceURI, null, null, null, null, null, null, null, 0,
					DateTime.Now, Guid.Empty.ToString(), "comment text", 1, show: true, section: null);

				var builder = _server.CreateRequest($"/consultations/api/Comment");
				builder.AddHeader(HeaderNames.Cookie, $"{Constants.SessionCookieName}{ConsultationId}={_sessionId}");
				builder.And(request =>
				{
					request.Content = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");
				});

				//Act
				var response = await builder.PostAsync();
				response.EnsureSuccessStatusCode();

				var responseString = await response.Content.ReadAsStringAsync();
				var deserialisedComment = JsonConvert.DeserializeObject<ViewModels.Comment>(responseString);

				var commentInDatabase = context.Comment.IgnoreQueryFilters().Single(dbComment => dbComment.CommentId.Equals(deserialisedComment.CommentId));

				// Assert
				response.StatusCode.ShouldBe(HttpStatusCode.Created);
				commentInDatabase.CreatedByUserId.ShouldBeNull();
				commentInDatabase.OrganisationUserId.ShouldBe(organisationUserId);
				commentInDatabase.ParentCommentId.ShouldBeNull();
				commentInDatabase.OrganisationId.ShouldBe(organisationId);
			}
		}
	}
}
