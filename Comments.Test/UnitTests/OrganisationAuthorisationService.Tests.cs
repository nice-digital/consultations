using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using NICE.Identity.Authentication.Sdk.Domain;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
				var serviceUnderTest = new OrganisationAuthorisationService(consultationsContext, userService);

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
				
				var serviceUnderTest = new OrganisationAuthorisationService(context, userService);

				//Act + Assert
				Assert.Throws<ApplicationException>(() => serviceUnderTest.GenerateOrganisationCode(organisationId, consultationId));
			}
		}

		[Fact]
		public void CollationCodeIsCorrectFormat()
		{
			//Arrange
			var serviceUnderTest = new OrganisationAuthorisationService(_context, _fakeUserService);
			const string validRegexForCollationCode = @"\d{4}\s\d{4}\s\d{4}"; //[4 numbers][space][4 numbers][space][4 numbers]
			var regex = new Regex(validRegexForCollationCode);

			//Act
			var collationCode = serviceUnderTest.GenerateOrganisationCode(1, 1);
			
			//Assert
			regex.IsMatch(collationCode).ShouldBeTrue();
		}

		[Fact]
		public void CollationCodeIsReturnedDifferentInRepeatedCalls()
		{
			//Arrange
			var serviceUnderTest = new OrganisationAuthorisationService(_context, _fakeUserService);
			const int numberOfTimesToGetCollationCode = 100;

			//Act
			var collationCodes = new List<string>();
			for (var counter = 0; counter < numberOfTimesToGetCollationCode; counter++)
			{
				collationCodes.Add(serviceUnderTest.GenerateOrganisationCode(1, consultationId: counter));
			}

			//Assert
			collationCodes.Distinct().Count().ShouldBe(numberOfTimesToGetCollationCode);
		}

		//[Fact]
		//public void CollationCodeDoesNotAlreadyExistInDatabase()
		//{
		//	//Arrange
		//	ResetDatabase();
		//	_context.Database.EnsureCreated();
		//	const int organisationId = 1;
		//	const int consultationId = 1;
		//	const string collationCode = "1234 1234 1234";

		//	var userService = new StubUserService(new User(isAuthorised: true, displayName: "Carl Spackler",
		//		userId: "001", new List<Organisation>() { new Organisation(organisationId, "Bushwood Country Club", isLead: true) }));

		//	using (var context = new ConsultationsContext(_options, userService, _fakeEncryption))
		//	{
		//		AddOrganisationAuthorisationWithLocation(2, consultationId, context, collationCode: collationCode);

		//		var serviceUnderTest = new OrganisationAuthorisationService(context, userService, _fakeHttpContextAccessor);

		//		//Act + Assert
		//		collationCode = serviceUnderTest.GenerateOrganisationCode(organisationId, consultationId);
		//	}
		//}
	}
}
