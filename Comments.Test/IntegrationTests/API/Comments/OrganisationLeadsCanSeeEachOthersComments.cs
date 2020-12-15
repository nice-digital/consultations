using Comments.Models;
using Comments.Test.Infrastructure;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Comments.Test.IntegrationTests.API.Comments
{
	public class OrganisationLeadsCanSeeEachOthersComments : TestBaseLight
	{
		private const int ConsultationId = 1;
		private const int OrganisationId = 1;
		private static readonly string SourceUri = "consultations://./consultation/" + ConsultationId + "/document/1";

		private const string OrganisationLead1UserId = "orglead1";
		private const string OrganisationLead2UserId = "orglead2";


		[Fact]
		public async Task OrganisationLead1CanSeeOrganisationLead2sComment()
		{
			//Arrange
			var context = new ConsultationsContext(GetContextOptions(), FakeUserService.Get(isAuthenticated: true, userId: OrganisationLead1UserId, displayName: "organisation lead 1", testUserType: TestUserType.Authenticated, organisationIdUserIsLeadOf: OrganisationId), new FakeEncryption());
			context.Database.EnsureDeleted();

			var locationId = TestBaseDBHelpers.AddLocation(context, SourceUri);
			var commentId = TestBaseDBHelpers.AddComment(context, locationId, "org lead 1's comment", OrganisationLead2UserId, 1, organisationId: OrganisationId);

			var locationId2 = TestBaseDBHelpers.AddLocation(context, SourceUri);
			TestBaseDBHelpers.AddComment(context, locationId2, "someone else's comment", "not one of the org leads", 1, organisationId: null);

			var locationId3 = TestBaseDBHelpers.AddLocation(context, SourceUri);
			TestBaseDBHelpers.AddComment(context, locationId3, "a comment by an org lead of a different organisation", "some other org lead", 1, organisationId: 99);

			//Act
			var dataReturned = context.GetAllCommentsAndQuestionsForDocument(new List<string>() {SourceUri}, false);

			//Assert
			dataReturned.Single().Comment.Single().CommentId.ShouldBe(commentId);
		}
	}
}
