using Comments.Common;
using Comments.Models;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Comments.Test.IntegrationTests.API.Comments
{
	public class OrganisationLeadsCanSeeEachOthersComments : TestBase
	{
		private const int OrganisationId = 1;
		private string commentText = Guid.NewGuid().ToString();

		private const string OrganisationLead1UserId = "orglead1";
		private const string OrganisationLead2UserId = "orglead2";

		[Fact]
		public void OrganisationLead1CanSeeOrganisationLead2sComment()
		{
			// Arrange
			ResetDatabase();

			var locationId = AddLocation("consultations://./consultation/1/document/1/chapter/introduction");
			AddComment(locationId, commentText, createdByUserId: OrganisationLead1UserId, organisationId: OrganisationId);

			locationId = AddLocation("consultations://./consultation/1/document/1");
			AddComment(locationId, commentText, createdByUserId: OrganisationLead2UserId, organisationId: OrganisationId);

			locationId = AddLocation("consultations://./consultation/1/document/2");
			AddComment(locationId, commentText, "Another user", organisationId: null);

			locationId = AddLocation("consultations://./consultation/1");
			AddComment(locationId, commentText, "Another org lead", organisationId: 99);

			var sourceURIs = new List<string>
			{
				ConsultationsUri.ConvertToConsultationsUri("/1/Review", CommentOn.Consultation)
			};

			var fakeUserService = FakeUserService.Get(isAuthenticated: true, displayName: "org lead 1", OrganisationLead1UserId, TestUserType.Authenticated, organisationIdUserIsLeadOf: OrganisationId);
			var consultationsContext = new ConsultationsContext(_options, fakeUserService, _fakeEncryption);

			// Act
			var results = consultationsContext.GetAllCommentsAndQuestionsForDocument(sourceURIs, true);

			//Assert
			results.Count().ShouldBe(2);
		}
	}
}
