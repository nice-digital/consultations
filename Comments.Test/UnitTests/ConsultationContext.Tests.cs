using Comments.Common;
using Comments.Models;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Comments.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Comments.Test.UnitTests
{
    public class ConsultationContext : TestBase
    {
        [Fact]
	    public void Get_All_Consultation_Comments_When_IsReview_Is_True()
	    {
		    // Arrange
		    ResetDatabase();
		    var commentText = Guid.NewGuid().ToString();
		    
		    var locationId = AddLocation("consultations://./consultation/1/document/1/chapter/introduction");
		    AddComment(locationId, commentText, _userId);

		    locationId = AddLocation("consultations://./consultation/1/document/1");
		    AddComment(locationId, commentText, _userId);

		    locationId = AddLocation("consultations://./consultation/1/document/2");
		    AddComment(locationId, commentText, _userId);

			locationId = AddLocation("consultations://./consultation/1");
		    AddComment(locationId, commentText, _userId);

		    locationId = AddLocation("consultations://./consultation/2");
		    AddComment(locationId, commentText, _userId);

		    locationId = AddLocation("consultations://./consultation/2");
		    AddComment(locationId, commentText, "Different user id");


		    var sourceURIs = new List<string>
		    {
			    ConsultationsUri.ConvertToConsultationsUri("/1/Review", CommentOn.Consultation)
		    };

			// Act
			var consultationsContext = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption);
		    var results = consultationsContext.GetAllCommentsAndQuestionsForDocument(sourceURIs, true);

			//Assert
			results.Count().ShouldBe(4);
	    }

	    [Fact]
	    public void Get_Chapter_Comments_When_IsReview_Is_False()
	    {
		    // Arrange
		    ResetDatabase();
		    var commentText = Guid.NewGuid().ToString();

		    var locationId = AddLocation("consultations://./consultation/1/document/1/chapter/introduction");
		    AddComment(locationId, commentText, _userId);

		    locationId = AddLocation("consultations://./consultation/1/document/1/chapter/overview");
		    AddComment(locationId, commentText, _userId);

			locationId = AddLocation("consultations://./consultation/1/document/1");
		    AddComment(locationId, commentText, _userId);

		    locationId = AddLocation("consultations://./consultation/1/document/2");
		    AddComment(locationId, commentText, _userId);

		    locationId = AddLocation("consultations://./consultation/1");
		    AddComment(locationId, commentText, _userId);

		    locationId = AddLocation("consultations://./consultation/2");
		    AddComment(locationId, commentText, _userId);


		    var sourceURIs = new List<string>
		    {
			    ConsultationsUri.ConvertToConsultationsUri("/1/1/Introduction", CommentOn.Consultation),
			    ConsultationsUri.ConvertToConsultationsUri("/1/1/Introduction", CommentOn.Document),
			    ConsultationsUri.ConvertToConsultationsUri("/1/1/Introduction", CommentOn.Chapter)
			};

		    // Act
		    var consultationsContext = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption);
		    var results = consultationsContext.GetAllCommentsAndQuestionsForDocument(sourceURIs, false);

		    //Assert
		    results.Count().ShouldBe(3);
	    }

	    [Fact]
	    public void Get_Questions_For_SourceURI()
	    {
		    // Arrange
		    ResetDatabase();
			_context.Database.EnsureCreated();

			var expectedQuestionIdsInResultSet = new List<int>();

			//these questions should appear in the resultset
		    var locationId = AddLocation("consultations://./consultation/1/document/1/chapter/introduction");
			var questionTypeId = 99;
		    expectedQuestionIdsInResultSet.Add(AddQuestion(locationId, questionTypeId, "Question Label"));		

			locationId = AddLocation("consultations://./consultation/1/document/1");
		    expectedQuestionIdsInResultSet.Add(AddQuestion(locationId, questionTypeId, "Question Label"));

			locationId = AddLocation("consultations://./consultation/1");
			expectedQuestionIdsInResultSet.Add(AddQuestion(locationId, questionTypeId, "Question Label"));

			//these questions shouldn't appear in the resultset.
		    var differetDocumentLocationId = AddLocation("consultations://./consultation/1/document/2");
		    AddQuestion(differetDocumentLocationId, questionTypeId, "Question Label");

			var differentChapterLocationId = AddLocation("consultations://./consultation/1/document/1/chapter/overview");
		    AddQuestion(differentChapterLocationId, questionTypeId, "Question Label");

			var differentConsultationLocationId = AddLocation("consultations://./consultation/2");
			AddQuestion(differentConsultationLocationId, questionTypeId, "Question Label");

			var sourceURIs = new List<string>
		    {
			    ConsultationsUri.ConvertToConsultationsUri("/1/1/Introduction", CommentOn.Consultation),
			    ConsultationsUri.ConvertToConsultationsUri("/1/1/Introduction", CommentOn.Document),
			    ConsultationsUri.ConvertToConsultationsUri("/1/1/Introduction", CommentOn.Chapter)
		    };

		    // Act
		    var consultationsContext = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption);
		    var results = consultationsContext.GetQuestionsForDocument(sourceURIs, false).ToList();
			
		    //Assert
		    results.Count.ShouldBe(expectedQuestionIdsInResultSet.Count);
		    foreach (var result in results)
		    {
			    foreach (var question in result.Question)
			    {
				    expectedQuestionIdsInResultSet.Contains(question.QuestionId).ShouldBeTrue();
			    }
		    }
		}

		[Fact]
		public void Get_Questions_For_Document_SourceURI()
		{
			// Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();

			var expectedQuestionIdsInResultSet = new List<int>();

			//these questions should appear in the resultset
			var locationId = AddLocation("consultations://./consultation/1/document/1/chapter/introduction");
			var questionTypeId = 99;
			expectedQuestionIdsInResultSet.Add(AddQuestion(locationId, questionTypeId, "Question Label"));

			locationId = AddLocation("consultations://./consultation/1/document/1");
			expectedQuestionIdsInResultSet.Add(AddQuestion(locationId, questionTypeId, "Question Label"));

			var differentChapterLocationId = AddLocation("consultations://./consultation/1/document/1/chapter/overview");
			expectedQuestionIdsInResultSet.Add(AddQuestion(differentChapterLocationId, questionTypeId, "Question Label"));

			//these questions shouldn't appear in the resultset.

			var consultationLevelLocationId = AddLocation("consultations://./consultation/1");
			AddQuestion(consultationLevelLocationId, questionTypeId, "Question Label");

			var differetDocumentLocationId = AddLocation("consultations://./consultation/1/document/2");
			AddQuestion(differetDocumentLocationId, questionTypeId, "Question Label");

			var differentConsultationLocationId = AddLocation("consultations://./consultation/2");
			AddQuestion(differentConsultationLocationId, questionTypeId, "Question Label");


			var sourceURIs = new List<string>
			{
			    ConsultationsUri.ConvertToConsultationsUri("/1/1/Introduction", CommentOn.Document),
		    };

			// Act
			var consultationsContext = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption);
			var results = consultationsContext.GetQuestionsForDocument(sourceURIs, true).ToList();

			//Assert
			results.Count.ShouldBe(expectedQuestionIdsInResultSet.Count);
			foreach (var result in results)
			{
				foreach (var question in result.Question)
				{
					expectedQuestionIdsInResultSet.Contains(question.QuestionId).ShouldBeTrue();
				}
			}
		}

		[Fact]
		public void Get_Questions_For_Consultation_SourceURI()
		{
			// Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();

			var expectedQuestionIdsInResultSet = new List<int>();

			var questionTypeId = 99;

			//these questions should appear in the resultset
			var consultationLevelLocationId = AddLocation("consultations://./consultation/1");
			expectedQuestionIdsInResultSet.Add(AddQuestion(consultationLevelLocationId, questionTypeId, "Question Label"));
			
			//these questions shouldn't appear in the resultset.

			var locationId = AddLocation("consultations://./consultation/1/document/1/chapter/introduction");
			AddQuestion(locationId, questionTypeId, "Question Label");

			locationId = AddLocation("consultations://./consultation/1/document/1");
			AddQuestion(locationId, questionTypeId, "Question Label");

			var differentChapterLocationId = AddLocation("consultations://./consultation/1/document/1/chapter/overview");
			AddQuestion(differentChapterLocationId, questionTypeId, "Question Label");

			var differetDocumentLocationId = AddLocation("consultations://./consultation/1/document/2");
			AddQuestion(differetDocumentLocationId, questionTypeId, "Question Label");

			var differentConsultationLocationId = AddLocation("consultations://./consultation/2");
			AddQuestion(differentConsultationLocationId, questionTypeId, "Question Label");


			var sourceURIs = new List<string>
			{
				ConsultationsUri.ConvertToConsultationsUri("/1/1/Introduction", CommentOn.Consultation),
			};

			// Act
			var consultationsContext = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption);
			var results = consultationsContext.GetQuestionsForDocument(sourceURIs, false).ToList();

			//Assert
			results.Count.ShouldBe(expectedQuestionIdsInResultSet.Count);
			foreach (var result in results)
			{
				foreach (var question in result.Question)
				{
					expectedQuestionIdsInResultSet.Contains(question.QuestionId).ShouldBeTrue();
				}
			}
		}


		[Fact]
		public void Get_Questions_For_Consultation_Partially_Matching_SourceURI()
		{
			// Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();

			var expectedQuestionIdsInResultSet = new List<int>();

			var questionTypeId = 99;

			//these questions should appear in the resultset
			var consultationLevelLocationId = AddLocation("consultations://./consultation/1");
			expectedQuestionIdsInResultSet.Add(AddQuestion(consultationLevelLocationId, questionTypeId, "Question Label"));

			var locationId = AddLocation("consultations://./consultation/1/document/1/chapter/introduction");
			expectedQuestionIdsInResultSet.Add(AddQuestion(locationId, questionTypeId, "Question Label"));

			locationId = AddLocation("consultations://./consultation/1/document/1");
			expectedQuestionIdsInResultSet.Add(AddQuestion(locationId, questionTypeId, "Question Label"));

			var differentChapterLocationId = AddLocation("consultations://./consultation/1/document/1/chapter/overview");
			expectedQuestionIdsInResultSet.Add(AddQuestion(differentChapterLocationId, questionTypeId, "Question Label"));

			var differetDocumentLocationId = AddLocation("consultations://./consultation/1/document/2");
			expectedQuestionIdsInResultSet.Add(AddQuestion(differetDocumentLocationId, questionTypeId, "Question Label"));

			//these questions shouldn't appear in the resultset.

			var differentConsultationLocationId = AddLocation("consultations://./consultation/2");
			AddQuestion(differentConsultationLocationId, questionTypeId, "Question Label");


			var sourceURIs = new List<string>
			{
				ConsultationsUri.ConvertToConsultationsUri("/1/1/Introduction", CommentOn.Consultation),
			};

			// Act
			var consultationsContext = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption);
			var results = consultationsContext.GetQuestionsForDocument(sourceURIs, true).ToList();

			//Assert
			results.Count.ShouldBe(expectedQuestionIdsInResultSet.Count);
			foreach (var result in results)
			{
				foreach (var question in result.Question)
				{
					expectedQuestionIdsInResultSet.Contains(question.QuestionId).ShouldBeTrue();
				}
			}
		}

		[Fact]
		public void Get_All_QuestionTypes_Where_There_Is_Only_A_Single_QuestionType()
		{
			// Arrange
			ResetDatabase();

			var textQuestionTypeId = AddQuestionType("Text", false, true);

			// Act
			var consultationsContext = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption);
			var results = consultationsContext.GetQuestionTypes();

			//Assert
			results.Single().QuestionTypeId.ShouldBe(textQuestionTypeId);
		}

	    [Fact]
	    public void Get_All_QuestionTypes_Where_There_Are_Multiple_QuestionTypes()
	    {
		    // Arrange
		    ResetDatabase();

		    var textQuestionTypeId = AddQuestionType("Text", hasBooleanAnswer: false, hasTextAnswer: true, questionTypeId: 1);
		    var booleanQuestionTypeId = AddQuestionType("Boolean", hasBooleanAnswer: true, hasTextAnswer: false, questionTypeId: 2);

			// Act
			var consultationsContext = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption);
		    var results = consultationsContext.GetQuestionTypes().ToList();

		    //Assert
			results.Count().ShouldBe(2);
		    results.First().QuestionTypeId.ShouldBe(textQuestionTypeId);
		    results.Skip(1).First().QuestionTypeId.ShouldBe(booleanQuestionTypeId);
		}

		[Fact]
		public void Get_Previous_Questions_Filtering_out_duplicates()
		{
			// Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();

			var questionTypeId = 99;
			var identicalQuestionText =
				"This question text is used in multiple questions. It should be distincted out so only 1 such question appears in the set";

			var locationId1 = AddLocation("consultations://./consultation/1");
			var questionId1 = AddQuestion(locationId1, questionTypeId, identicalQuestionText);

			var locationId2 = AddLocation("consultations://./consultation/2");
			AddQuestion(locationId2, questionTypeId, identicalQuestionText);

			var questionId2 = AddQuestion(locationId2, questionTypeId, "unique question");
			
			// Act
			var consultationsContext = new ConsultationsContext(_options, _fakeUserService, _fakeEncryption);
			var results = consultationsContext.GetAllPreviousUniqueQuestions().ToList();

			//Assert
			results.Count.ShouldBe(2);
			results[0].QuestionId.ShouldBe(questionId1);
			results[1].QuestionId.ShouldBe(questionId2);
		}

		[Fact]
		public void Get_Previous_Questions_Filtering_out_deletions()
		{
			// Arrange
			ResetDatabase();

			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: Guid.NewGuid().ToString());
			var context = CreateContext(userService);
			context.Database.EnsureCreated();

			var questionTypeId = 99;

			var locationId = AddLocation("consultations://./consultation/1", context);

			var questionId1 = AddQuestion(locationId, questionTypeId, "question 1", context);
			var questionId2 = AddQuestion(locationId, questionTypeId, "question 2", context);

			var question2 = context.GetQuestion(questionId2);
			question2.IsDeleted = true;

			// Act
			var results = context.GetAllPreviousUniqueQuestions().ToList();

			//Assert
			results.Single().QuestionId.ShouldBe(questionId1);
		}

		[Fact]
		public void GetAllSourceURIsTheCurrentUserHasCommentedOrAnsweredAQuestion_returns_current_users_only()
		{
			// Arrange
			ResetDatabase();
			_context.Database.EnsureCreated();

			var chapterLevelSourceURI = "consultations://./consultation/1/document/1/chapter/intro";
			var anotherChapterLevelSourceURI = "consultations://./consultation/1/document/1/chapter/nextchap";
			var anotherConsultationSourceURI = "consultations://./consultation/2";

			var sourceURINotUsedByCurrentUser = "consultations://./consultation/1/document/1";

			var createdByUserId = Guid.Empty.ToString();
			var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: createdByUserId);

			///source uri's that should be in the resultset:
			var chapterLocationId = AddLocation(chapterLevelSourceURI, _context);
			AddComment(chapterLocationId, "", createdByUserId);
			var questionId = AddQuestion(chapterLocationId, 99, "", _context);
			AddAnswer(questionId, createdByUserId, "", 1, _context);

			var anotherChapterLevelSourceURILocationId = AddLocation(anotherChapterLevelSourceURI, _context);
			AddComment(anotherChapterLevelSourceURILocationId, "", createdByUserId);

			var anotherConsultationSourceURILocationId = AddLocation(anotherConsultationSourceURI, _context);
			AddComment(anotherConsultationSourceURILocationId, "", createdByUserId, (int)StatusName.Submitted);

			//source uri that shouldn't be in the resultset.
			var sourceURINotUsedByCurrentUserLocationId = AddLocation(sourceURINotUsedByCurrentUser, _context);
			AddComment(sourceURINotUsedByCurrentUserLocationId, "", Guid.NewGuid().ToString()); //note the user is random.
			
			var context = new ConsultationsContext(_options, userService, _fakeEncryption);

			// Act    
			var data = context.GetAllSourceURIsTheCurrentUserHasCommentedOrAnsweredAQuestion().ToList();

			//Assert
			data.Count(x => x.Key.Equals(chapterLevelSourceURI)).ShouldBe(2); //1 for the comment another 1 for the answer
			data.Count(x => x.Key.Equals(anotherChapterLevelSourceURI)).ShouldBe(1); //1 for comment on same consultation
			data.Count(x => x.Key.Equals(anotherConsultationSourceURI)).ShouldBe(1); //1 for comment on different consultation

			data.First(x => x.Key.Equals(chapterLevelSourceURI)).Value.StatusId.ShouldBe((int)StatusName.Draft); //now check the status returned
			data.Where(x => x.Key.Equals(chapterLevelSourceURI)).Skip(1).Single().Value.StatusId.ShouldBe((int)StatusName.Draft); 
			data.Single(x => x.Key.Equals(anotherChapterLevelSourceURI)).Value.StatusId.ShouldBe((int)StatusName.Draft);
			data.Single(x => x.Key.Equals(anotherConsultationSourceURI)).Value.StatusId.ShouldBe((int)StatusName.Submitted);

			data.Count(x => x.Key.Equals(sourceURINotUsedByCurrentUser)).ShouldBe(0);
		}


		[Fact]
		public void UpdateEmailAddressForOrganisationUser()
		{
			//Arrange
			var consultationId = 1;
			var organisationId = 1;
			var authorisationSession = Guid.NewGuid();
			var emailAddress = "email@test.com";

			var organisationAuthorisationId = TestBaseDBHelpers.AddOrganisationAuthorisationWithLocation(organisationId, consultationId, _context);
			var organisationUserId = TestBaseDBHelpers.AddOrganisationUser(_context, organisationAuthorisationId, authorisationSession, null);

			var userService = FakeUserService.Get(true, "Benjamin Button", null, TestUserType.NotAuthenticated, false, organisationUserId, null);
			var context = new ConsultationsContext(_options, userService, _fakeEncryption);

			//Act
			var OrgUser = context.UpdateEmailAddressForOrganisationUser(emailAddress, organisationUserId);

			//Assert
			OrgUser.EmailAddress.ShouldBe(emailAddress);
		}
    
    		public void Get_Other_Org_Users_Comments()
		{
			// Arrange
			ResetDatabase();
			var commentText = Guid.NewGuid().ToString();
			var myOrganisationId = 1;
			var anotherOrganisationId = 2;
			var myOrganisationUserId = 1;
			var anotherOrganisationUserId = 2;
			var sourceURI = "consultations://./consultation/1";

			// My comment
			var locationId = AddLocation(sourceURI);
			AddComment(locationId, commentText, null, status: (int)StatusName.SubmittedToLead, organisationUserId: myOrganisationUserId, organisationId: myOrganisationId);

			// Another user in my organisation's submitted comment
			AddComment(locationId, commentText, null, status: (int)StatusName.SubmittedToLead, organisationUserId: anotherOrganisationUserId, organisationId: myOrganisationId);

			// Another user in my organisation's draft comment
			AddComment(locationId, commentText, null, status: (int)StatusName.Draft, organisationUserId: anotherOrganisationUserId, organisationId: myOrganisationId);

			// A user from a different organisation's comment
			AddComment(locationId, commentText, null, status: (int)StatusName.SubmittedToLead, organisationUserId: anotherOrganisationUserId, organisationId: anotherOrganisationId);

			var sourceURIs = new List<string>
			{
				ConsultationsUri.ConvertToConsultationsUri(sourceURI, CommentOn.Consultation)
			};

			// Act
			var userService = FakeUserService.Get(isAuthenticated: false, organisationUserId: myOrganisationUserId);
			var consultationsContext = new ConsultationsContext(_options, userService, _fakeEncryption);
			var results = consultationsContext.GetOtherOrganisationUsersCommentsAndQuestionsForDocument(sourceURIs);

			//Assert
			results.Count().ShouldBe(1);
		}

		[Fact]

		public void Get_All_Comments_And_Answers_Submitted_To_A_Lead_For_Given_Organisation()
		{
			//Arrange
			var organisationId = 1;
			var sourceURI = "consultations://./consultation/1";
			var context = AddCommentsAndAnwersToALead(sourceURI, organisationId);

			//Act
			var (comment, answer) = context.GetCommentsAndAnswersSubmittedToLeadForOrganisation(organisationId);

			//Assert
			comment.Count.ShouldBe(2);
			answer.Count.ShouldBe(2);
		}

		[Fact]
		public void Count_Comments_And_Answers_Submitted_To_A_Lead_For_Given_Organisation()
		{
			//Arrange
			var organisationId = 1;
			var sourceURI = "consultations://./consultation/1";
			var context = AddCommentsAndAnwersToALead(sourceURI, organisationId);

			//Act
			var count = context.CountCommentsAndAnswerSubmissionsForThisOrganisation(sourceURI, organisationId);

			//Assert
			count.ShouldBe(2);
		}

		#region "Add test data"
		/// <summary>
		/// Setup test data, for answer and comment add
		///  - one submitted to lead item for organisation
		///  - one draft item for the same organisation
		///  - one submitted to lead item for a different organisation
		///  - one submitted to lead item but for a different consultation
		/// </summary>
		/// <param name="sourceURI"></param>
		/// <param name="organisationId"></param>
		/// <returns>context</returns>
		private ConsultationsContext AddCommentsAndAnwersToALead(string sourceURI, int organisationId)
		{
			var consultationId = 1;
			var authorisationSession = Guid.NewGuid();

			var organisationAuthorisationId = TestBaseDBHelpers.AddOrganisationAuthorisationWithLocation(organisationId, consultationId, _context);
			var organisationUserId = TestBaseDBHelpers.AddOrganisationUser(_context, organisationAuthorisationId, authorisationSession, null);

			var differentOrganisationAuthorisationId = TestBaseDBHelpers.AddOrganisationAuthorisationWithLocation(organisationId + 1, consultationId, _context);
			var differentOrganisationUserId = TestBaseDBHelpers.AddOrganisationUser(_context, differentOrganisationAuthorisationId, authorisationSession, null);

			var userService = FakeUserService.Get(true, "Benjamin Button", null, TestUserType.NotAuthenticated, false, organisationUserId, null);
			var context = new ConsultationsContext(_options, userService, _fakeEncryption);

			var locationId = AddLocation(sourceURI);
			var LocationIdForDifferentConsulation = AddLocation("consultations://./consultation/2");

			AddComment(locationId, "Comment Text", null, (int)StatusName.SubmittedToLead, context, organisationUserId);
			AddComment(locationId, "Comment Text", null, (int)StatusName.Draft, context, organisationUserId);
			AddComment(locationId, "Comment Text", null, (int)StatusName.SubmittedToLead, context, differentOrganisationUserId);
			AddComment(LocationIdForDifferentConsulation, "Comment Text", null, (int)StatusName.SubmittedToLead, context, organisationUserId);

			var questionId = AddQuestion(locationId, 99, "Question Text", context);
			var QuestionIdForDifferentConsultation = AddQuestion(LocationIdForDifferentConsulation, 99, "Question Text", context);
			AddAnswer(questionId, null, "Answer Text", (int)StatusName.SubmittedToLead, context, organisationUserId);
			AddAnswer(questionId, null, "Answer Text", (int)StatusName.Draft, context, organisationUserId);
			AddAnswer(questionId, null, "Answer Text", (int)StatusName.SubmittedToLead, context, differentOrganisationUserId);
			AddAnswer(QuestionIdForDifferentConsultation, null, "Answer Text", (int)StatusName.SubmittedToLead, context, organisationUserId);

			return context;
		}
		#endregion
	}
}
