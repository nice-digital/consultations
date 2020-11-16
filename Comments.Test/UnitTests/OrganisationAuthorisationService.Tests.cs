using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Shouldly;
using System;
using Comments.ViewModels;
using Xunit;

namespace Comments.Test.UnitTests
{
	public class OrganisationAuthorisationServiceTests : TestBase
    {

		//[Fact]
		//public void UserIsLeadOfOrganisationPassedIn()
		//{
		//	//Arrange
		//	ResetDatabase();
		//	_context.Database.EnsureCreated();
		//	const int organisationId = 1;

		//	var userService = new StubUserService(new User(isAuthorised: true, displayName: "Carl Spackler", userId: "1", organisationName: "Bushwood Country Club"));
			
		//	using (var consultationsContext = new ConsultationsContext(_options, userService, _fakeEncryption))
		//	{
		//		var serviceUnderTest = new OrganisationAuthorisationService(consultationsContext, userService, _fakeHttpContextAccessor);


		//		//Act
		//		var returnedCollationCode = serviceUnderTest.GenerateCollationCode(1, 1);

		//		//Assert
		//		returnedCollationCode.ShouldNotBe(existingCollationCode);
		//	}
		//}


		//  [Fact]
		//     public void CollationCodeNotInDatabaseAlready()
		//     {
		////Arrange
		//ResetDatabase();
		//_context.Database.EnsureCreated();
		//const string existingCollationCode = "existing code";

		//using (var consultationsContext = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption))
		//{
		//	consultationsContext.OrganisationAuthorisation.Add(
		//		new OrganisationAuthorisation(null, DateTime.Now, 1, 1, existingCollationCode));
		//	consultationsContext.SaveChanges();

		//	var serviceUnderTest = new OrganisationAuthorisationService(consultationsContext, _fakeUserService, _fakeHttpContextAccessor);


		//	//Act
		//	var returnedCollationCode = serviceUnderTest.GenerateCollationCode(1, 1);

		//	//Assert
		//	returnedCollationCode.ShouldNotBe(existingCollationCode);
		//}
		//     }
	}
}



