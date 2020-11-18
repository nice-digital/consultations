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
			var collationCode = serviceUnderTest.GenerateOrganisationCode(1, 1).CollationCode;
			
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
				collationCodes.Add(serviceUnderTest.GenerateOrganisationCode(1, consultationId: counter).CollationCode);
			}

			//Assert
			collationCodes.Distinct().Count().ShouldBe(numberOfTimesToGetCollationCode);
		}

		[Fact]
		public void CollationCodeGetsSavedInDatabase()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			const int organisationId = 1;
			const int consultationId = 1;

			var userService = new StubUserService(new User(isAuthorised: true, displayName: "Carl Spackler",
				userId: "001", new List<Organisation> { new Organisation(organisationId, "Bushwood Country Club", isLead: true) }));

			using (var context = new ConsultationsContext(_options, userService, _fakeEncryption))
			{
				var serviceUnderTest = new OrganisationAuthorisationService(context, userService);

				//Act 
				serviceUnderTest.GenerateOrganisationCode(organisationId, consultationId);

				//Assert
				context.OrganisationAuthorisation.Count().ShouldBe(1);
			}
		}

		[Theory]
		[InlineData("123412341234", 1)]
		[InlineData("1234 1234 1234", 1)] //the user is going to be shown the code like this, chunked into 4 digit groups.
		[InlineData("12 34 12 34 12 34", 1)]
		[InlineData("1 2 3 4 1 2 3 4 1 2 3 4", 1)] //but we ignore all spaces, if they choose to add some more.
		public void CheckValidCodeForConsultationReturnsValid(string collationCode, int consultationId)
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			const string collationCodeInDB = "123412341234";
			const int organisationId = 1;

			using (var context = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption))
			{
				AddOrganisationAuthorisationWithLocation(organisationId, consultationId, context, collationCode: collationCodeInDB);
				var serviceUnderTest = new OrganisationAuthorisationService(context, _fakeUserService);

				//Act 
				var organisationCode = serviceUnderTest.CheckValidCodeForConsultation(collationCode, consultationId);

				//Assert
				organisationCode.ShouldNotBeNull();
			}
		}

		[Theory]
		[InlineData("111111111111", 1)]
		[InlineData("123412341234", 2)]
		[InlineData("1234", 1)]
		[InlineData("1234123412341", 1)]
		public void CheckValidCodeForConsultationReturnsInvalid(string collationCode, int consultationId)
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			const string collationCodeInDB = "123412341234";
			const int organisationId = 1;
			const int validConsultationId = 1;

			using (var context = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption))
			{
				AddOrganisationAuthorisationWithLocation(organisationId, validConsultationId, context, collationCode: collationCodeInDB);
				var serviceUnderTest = new OrganisationAuthorisationService(context, _fakeUserService);

				//Act 
				var organisationCode = serviceUnderTest.CheckValidCodeForConsultation(collationCode, consultationId);

				//Assert
				organisationCode.ShouldBeNull();
			}
		}
	}
}
