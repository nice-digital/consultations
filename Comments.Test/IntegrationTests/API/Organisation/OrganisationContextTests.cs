using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Comments.Models;
using Comments.Test.Infrastructure;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Comments.Test.IntegrationTests.API.Organisation
{
	public class OrganisationContextTests : TestBaseLight
	{
		private static readonly Guid _sessionId = Guid.Parse("11111111-1111-1111-1111-111111111111");
		private readonly int consultationId = 1;

		[Fact]
		public void GetAllCommentsAndQuestionsForDocumentCanHandleOrganisationAuthorisations()
		{
			const int organisationUserId = 1;
			const int organisationId = 1;
			var context = new ConsultationsContext(GetContextOptions(), FakeUserService.Get(isAuthenticated: true, displayName: "Carl Spackler", userId: "Carl", testUserType: TestUserType.Authenticated, organisationIdUserIsLeadOf: organisationId), new FakeEncryption());
			context.Database.EnsureDeleted();

			var sourceURI = $"consultations://./consultation/{consultationId}";

			var organisationAuthorisationId = TestBaseDBHelpers.AddOrganisationAuthorisationWithLocation(organisationId, consultationId, context, null, "123412341234");

			//var locationId = TestBaseDBHelpers.AddLocation(context, sourceURI);
			//TestBaseDBHelpers.AddComment(context, locationId, "comment text", createdByUserId: null, organisationUserId: organisationUserId);

			//Act
			var locations = context.GetAllCommentsAndQuestionsForDocument(new List<string>() {sourceURI}, true);

			// Assert
			locations.Count().ShouldBe(0);
		}

	}
}
