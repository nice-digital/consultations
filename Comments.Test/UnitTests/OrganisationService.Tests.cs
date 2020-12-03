using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using NICE.Identity.Authentication.Sdk.Domain;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Comments.Common;
using Moq;
using Xunit;

namespace Comments.Test.UnitTests
{
	public class OrganisationServiceTests : TestBase
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
				var serviceUnderTest = new OrganisationService(consultationsContext, userService, null, null, null, null);

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
				
				var serviceUnderTest = new OrganisationService(context, userService, null, null, null, null);

				//Act + Assert
				Assert.Throws<ApplicationException>(() => serviceUnderTest.GenerateOrganisationCode(organisationId, consultationId));
			}
		}

		[Fact]
		public void CollationCodeIsCorrectFormat()
		{
			//Arrange
			var serviceUnderTest = new OrganisationService(_context, _fakeUserService, null, null, null, null);
			var regex = new Regex(Constants.CollationCode.RegExChunkedWithSpaces);

			//Act
			var collationCode = serviceUnderTest.GenerateOrganisationCode(1, 1).CollationCode;
			
			//Assert
			regex.IsMatch(collationCode).ShouldBeTrue();
		}

		[Fact]
		public void CollationCodeIsReturnedDifferentInRepeatedCalls()
		{
			//Arrange
			var serviceUnderTest = new OrganisationService(_context, _fakeUserService, null, null, null, null);
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
				var serviceUnderTest = new OrganisationService(context, userService, null, null, null, null);

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
		public async void CheckValidCodeForConsultationReturnsValid(string collationCode, int consultationId)
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			const string collationCodeInDB = "123412341234";
			const int organisationId = 1;
			var mockFactory = new Mock<IHttpClientFactory>();

			using (var context = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption))
			{
				AddOrganisationAuthorisationWithLocation(organisationId, consultationId, context, collationCode: collationCodeInDB);
				var serviceUnderTest = new OrganisationService(context, _fakeUserService, new FakeAPITokenService(), _fakeApiService, mockFactory.Object, null);

				//Act 
				var organisationCode = await serviceUnderTest.CheckValidCodeForConsultation(collationCode, consultationId);

				//Assert
				organisationCode.ShouldNotBeNull();
			}
		}

		[Theory]
		[InlineData("111111111111", 1)]
		[InlineData("123412341234", 2)]
		[InlineData("1234", 1)]
		[InlineData("1234123412341", 1)]
		public async Task CheckValidCodeForConsultationReturnsInvalid(string collationCode, int consultationId)
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
				var serviceUnderTest = new OrganisationService(context, _fakeUserService, null, null, null, null);

				//Act + Assert
				await Assert.ThrowsAsync<ApplicationException>(() => serviceUnderTest.CheckValidCodeForConsultation(collationCode, consultationId));
			}
		}

		[Fact]
		public async Task CheckValidCodeThrowsErrorWhenTheOrganisationHasAlreadySubmittedACommentForThisConsultationToNICE()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			const string collationCodeInDB = "123412341234";
			const int organisationId = 1;
			const int validConsultationId = 1;

			using (var context = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption))
			{
				var organisationAuthorisationId = AddOrganisationAuthorisationWithLocation(organisationId, validConsultationId, context, collationCode: collationCodeInDB);
				var organisationUserId = AddOrganisationUser(context, organisationAuthorisationId, Guid.NewGuid(), null);
				
				var sourceURI = ConsultationsUri.CreateConsultationURI(validConsultationId);
				var commentLocationId = AddLocation(sourceURI, context);
				var orgCommentersCommentId = AddComment(commentLocationId, "comment text", "Carl Spackler", (int)StatusName.Submitted, context, organisationUserId);

				AddComment(commentLocationId, "org lead comment text", "Judge Smails", (int)StatusName.Submitted, context, organisationUserId: null, parentCommentId: orgCommentersCommentId);

				var serviceUnderTest = new OrganisationService(context, _fakeUserService, null, null, null, null);

				//Act + Assert
				await Assert.ThrowsAsync<ApplicationException>(() => serviceUnderTest.CheckValidCodeForConsultation(collationCodeInDB, validConsultationId));
			}
		}

		[Fact]
		public async Task CheckValidCodeThrowsErrorWhenTheOrganisationHasAlreadySubmittedAnAnswerForThisConsultationToNICE()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			const string collationCodeInDB = "123412341234";
			const int organisationId = 1;
			const int validConsultationId = 1;

			using (var context = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption))
			{
				var organisationAuthorisationId = AddOrganisationAuthorisationWithLocation(organisationId, validConsultationId, context, collationCode: collationCodeInDB);
				var organisationUserId = AddOrganisationUser(context, organisationAuthorisationId, Guid.NewGuid(), null);

				var sourceURI = ConsultationsUri.CreateConsultationURI(validConsultationId);
				var answerLocationId = AddLocation(sourceURI, context);
				var questionId = AddQuestion(answerLocationId, 1, "question text", context);

				var orgCommentersAnswerId = AddAnswer(questionId, "Carl Spackler", "answer text", (int)StatusName.Submitted ,context, organisationUserId);

				AddAnswer(questionId, "Judge Smails", "answer text", (int)StatusName.Submitted, context, organisationUserId: null, parentAnswerId: orgCommentersAnswerId);

				var serviceUnderTest = new OrganisationService(context, _fakeUserService, null, null, null, null);

				//Act + Assert
				await Assert.ThrowsAsync<ApplicationException>(() => serviceUnderTest.CheckValidCodeForConsultation(collationCodeInDB, validConsultationId));
			}
		}

		[Fact]
		public void OrganisationUserCreatesAUniqueSessionIdWhenCalledMultipleTimesAndAllSessionsAreSavedToDatabaseAndReturned()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			const int numberOfSessionsToCreate = 100;
			var sessionsReturned = new Dictionary<Guid, DateTime>();
			var consultationEndDate = DateTime.ParseExact("25-Nov-2020", "dd-MMM-yyyy", DateTimeFormatInfo.InvariantInfo);
			var expectedExpirationDate = consultationEndDate.AddDays(28);

			using (var context = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption))
			{
				var serviceUnderTest = new OrganisationService(context, _fakeUserService, null, null, null, new FakeConsultationService(consultationEndDate: consultationEndDate));

				//Act
				for (var counter = 1; counter <= numberOfSessionsToCreate; counter++)
				{
					var collationCodeToUse = counter.ToString("000000000000");

					var organisationAuthorisationId = AddOrganisationAuthorisationWithLocation(1, 1, _context, collationCode: collationCodeToUse);

					var session = serviceUnderTest.CreateOrganisationUserSession(organisationAuthorisationId, collationCodeToUse);
					sessionsReturned.Add(session.sessionId, session.expirationDate);
				}

				var uniqueSessionIdsReturned = sessionsReturned.Keys.ToList();

				//Assert
				var sessionIdsInDatabase = _context.OrganisationUser.Select(ou => ou.AuthorisationSession).ToList();
				sessionIdsInDatabase.Count().ShouldBe(numberOfSessionsToCreate);

				var inDatabaseButNotReturned = sessionIdsInDatabase.Except(uniqueSessionIdsReturned);
				inDatabaseButNotReturned.Count().ShouldBe(0);

				var returnedButNotInDatabase = uniqueSessionIdsReturned.Except(sessionIdsInDatabase);
				returnedButNotInDatabase.Count().ShouldBe(0);

				sessionsReturned.First().Value.ShouldBe(expectedExpirationDate);
			}
		}

		[Fact]
		public void CheckOrganisationUserSessionRecognisesValidAndInvalidSessions()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var sessionId = Guid.NewGuid();
			var consultationId = 1;

			using (var context = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption))
			{
				var serviceUnderTest = new OrganisationService(context, _fakeUserService, null, null, null, null);

				var organisationAuthorisationId = AddOrganisationAuthorisationWithLocation(1, consultationId, context, collationCode: "123412341234");
				AddOrganisationUser(context, organisationAuthorisationId, sessionId, null);

				//Act
				var valid = serviceUnderTest.CheckOrganisationUserSession(consultationId, sessionId);

				var invalid1 = serviceUnderTest.CheckOrganisationUserSession(consultationId, Guid.NewGuid());
				var invalid2 = serviceUnderTest.CheckOrganisationUserSession(2, sessionId);
				var invalid3 = serviceUnderTest.CheckOrganisationUserSession(2, Guid.NewGuid());

				//Assert
				valid.ShouldBe(true);
				invalid1.ShouldBe(false);
				invalid2.ShouldBe(false);
				invalid3.ShouldBe(false);
			}
		}

		[Fact]
		public void CheckOrganisationUserSessionRecognisesNonExpiredSessionCorrectly()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var sessionId = Guid.NewGuid();
			var consultationId = 1;
			var expirationDate = DateTime.UtcNow.AddDays(1);

			using (var context = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption))
			{
				var serviceUnderTest = new OrganisationService(context, _fakeUserService, null, null, null, null);

				var organisationAuthorisationId = AddOrganisationAuthorisationWithLocation(1, consultationId, context, collationCode: "123412341234");
				AddOrganisationUser(context, organisationAuthorisationId, sessionId, expirationDate);

				//Act
				var valid = serviceUnderTest.CheckOrganisationUserSession(consultationId, sessionId);
				
				//Assert
				valid.ShouldBe(true);
			}
		}

		[Fact]
		public void CheckOrganisationUserSessionRecognisesExpiredSessionCorrectly()
		{
			//Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();
			var sessionId = Guid.NewGuid();
			var consultationId = 1;
			var expirationDate = DateTime.UtcNow.AddDays(-1);

			using (var context = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption))
			{
				var serviceUnderTest = new OrganisationService(context, _fakeUserService, null, null, null, null);

				var organisationAuthorisationId = AddOrganisationAuthorisationWithLocation(1, consultationId, context, collationCode: "123412341234");
				AddOrganisationUser(context, organisationAuthorisationId, sessionId, expirationDate);

				//Act
				var invalid = serviceUnderTest.CheckOrganisationUserSession(consultationId, sessionId);

				//Assert
				invalid.ShouldBe(false);
			}
		}
	}
}