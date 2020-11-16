using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Shouldly;
using System;
using System.Collections.Generic;
using Comments.ViewModels;
using Microsoft.AspNetCore.Http;
using NICE.Identity.Authentication.Sdk.Domain;
using Xunit;

namespace Comments.Test.UnitTests
{
	public class OrganisationAuthorisationServiceTests : TestBase
    {

		[Fact]
		public void UserIsLeadOfOrganisationPassedIn()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			const int organisationId = 1;

			var userService = new StubUserService(new User(isAuthorised: true, displayName: "Carl Spackler",
				userId: "001", new List<Organisation>() {new Organisation(organisationId, "Bushwood Country Club", isLead: false)})); 

			using (var consultationsContext = new ConsultationsContext(_options, userService, _fakeEncryption))
			{
				var serviceUnderTest = new OrganisationAuthorisationService(consultationsContext, userService, _fakeHttpContextAccessor);

				//Act + Assert
				Assert.Throws<UnauthorizedAccessException>(() => serviceUnderTest.GenerateOrganisationCode(organisationId, consultationId: 1));
			}
		}

		[Fact]
		public void OrganisationAlreadyHasACollationCodeForThisConsultation()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			const int organisationId = 1;
			const int consultationId = 1;

			var userService = new StubUserService(new User(isAuthorised: true, displayName: "Carl Spackler",
				userId: "001", new List<Organisation>() { new Organisation(organisationId, "Bushwood Country Club", isLead: true) }));

			using (var context = new ConsultationsContext(_options, userService, _fakeEncryption))
			{
				AddOrganisationAuthorisationWithLocation(organisationId, consultationId, context);
				
				var serviceUnderTest = new OrganisationAuthorisationService(context, userService, _fakeHttpContextAccessor);

				//Act + Assert
				Assert.Throws<ApplicationException>(() => serviceUnderTest.GenerateOrganisationCode(organisationId, consultationId));
			}
		}

		
	}

	//public class CollationCodeTests : OrganisationAuthorisationService
	//{
	//	public CollationCodeTests() : base(context, userService, httpContextAccessor)
	//	{
	//	}
	//}
}
