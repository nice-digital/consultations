using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Comments.Common;
using Comments.Models;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Comments.Test.IntegrationTests.API.Comments
{
	public class CreateCommentAsOrganisationalLead : TestBaseLight
	{
		private static readonly Guid _sessionId = Guid.Parse("11111111-1111-1111-1111-111111111111");
		private const int consultationId = 1;

		[Fact]
		public async Task Create_Comment_As_Lead_Sets_Comment_Fields_Correctly()
		{
			//Arrange

			const int organisationId = 1;
			var orgLeadUserId = Guid.NewGuid().ToString();
			var uniqueCommentText = Guid.NewGuid().ToString();

			var fakeUserService = FakeUserService.Get(isAuthenticated: true, userId: orgLeadUserId, displayName: "The Judge", testUserType: TestUserType.Authenticated, organisationUserId: null, organisationIdUserIsLeadOf: organisationId);

			using (var context = new ConsultationsContext(GetContextOptions(), fakeUserService, new FakeEncryption()))
			{
				context.Database.EnsureDeleted();

				var sourceURI = $"consultations://./consultation/{consultationId}/document/1";

				var organisationAuthorisationId = TestBaseDBHelpers.AddOrganisationAuthorisationWithLocation(organisationId, consultationId, context, null, "123412341234");
				
				var locationId = TestBaseDBHelpers.AddLocation(context, sourceURI);
				var commentId = TestBaseDBHelpers.AddComment(context, locationId, "comment text", createdByUserId: orgLeadUserId);

				var (_server, _client) = InitialiseServerAndClient(context, fakeUserService);

				var comment = new ViewModels.Comment(locationId, sourceURI, null, null, null, null, null, null, null, 0,
					DateTime.Now, Guid.Empty.ToString(), uniqueCommentText, 1, show: true, section: null);

				var builder = _server.CreateRequest($"/consultations/api/Comment");
				builder.And(request =>
				{
					request.Content = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");
				});

				//Act
				var response = await builder.PostAsync();
				response.EnsureSuccessStatusCode();

				var commentInDatabase = context.Comment.IgnoreQueryFilters().Single(dbComment => dbComment.CommentText.Equals(uniqueCommentText));

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
