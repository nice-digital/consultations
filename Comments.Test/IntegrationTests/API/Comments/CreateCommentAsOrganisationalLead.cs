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
using Xunit;

namespace Comments.Test.IntegrationTests.API.Comments
{
	public class CreateCommentAsOrganisationalLead : TestBaseLight
	{
		private const int consultationId = 1;

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
				var sourceURI = $"consultations://./consultation/{consultationId}/document/1";
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
	}
}
