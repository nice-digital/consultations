using System;
using System.Collections.Generic;
using Comments.Test.Infrastructure;
using System.Threading.Tasks;
using Comments.Models;
using Xunit;

namespace Comments.Test.IntegrationTests.API.OrganisationAuthorisation
{
	public class OrganisationAuthorisationTests : TestBase
	{
		
		public OrganisationAuthorisationTests(bool useRealSubmitService = false, TestUserType testUserType = TestUserType.Authenticated, bool useFakeConsultationService = false, IList<SubmittedCommentsAndAnswerCount> submittedCommentsAndAnswerCounts = null, bool bypassAuthentication = true, bool addRoleClaim = true)
			: base(useRealSubmitService, testUserType, useFakeConsultationService, submittedCommentsAndAnswerCounts, bypassAuthentication, addRoleClaim, true, organisationIdUserIsLeadOf: 1)
		{
		}

		[Fact]
		public async Task GenerateCode()
		{
			//Arrange (in the base constructor for this one.)

			// Act
			var response = await _client.PostAsync("/consultations/api/Organisation?organisationId=1&consultationId=1", null);
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();

			// Assert
			responseString.ShouldMatchApproved(new Func<string, string>[]{ Scrubbers.ScrubCollationCode, Scrubbers.ScrubOrganisationAuthorisationId });
		}
	}
}
