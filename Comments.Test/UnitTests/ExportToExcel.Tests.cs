using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Comments.Common;
using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Shouldly;
using Xunit;
using TestBase = Comments.Test.Infrastructure.TestBase;

namespace Comments.Test.UnitTests
{
    public class ExportToExcelTests : TestBase
    {
		[Fact]
		public async Task Create_Spreadsheet()
	    {
			// Arrange
		    ResetDatabase();
		    _context.Database.EnsureCreated();
		    CreateALotOfData();
		    var consultationId = 1;
			

			// Act
			var response = await _client.GetAsync($"consultations/api/Export/{consultationId}");

			//Assert
			response.IsSuccessStatusCode.ShouldBeTrue();
		}

	    private void CreateALotOfData()
	    {
		    var userId = Guid.NewGuid();
		    var sourceURI = "consultations://./consultation/1/document/2/chapter/introduction";
		    int locationId;

		    var userService = FakeUserService.Get(isAuthenticated: true, displayName: "Benjamin Button", userId: userId);
		    //var context = new ConsultationsContext(_options, userService, _fakeEncryption);
		    var authenticateService = new FakeAuthenticateService(authenticated: true);

			//AddCommentsAndQuestionsAndAnswers(sourceURI, "My Comment Text", "1 My Question Text", "My Answer Text", userId, (int)StatusName.Submitted, _context);
			//AddCommentsAndQuestionsAndAnswers(sourceURI, "A second comment", "2 A second question", "A second answer", userId, (int)StatusName.Submitted, _context);
			//AddCommentsAndQuestionsAndAnswers(sourceURI, "Yet another comment", "3 Yet another question", "Yet another answer", userId, (int)StatusName.Submitted, _context);
			//AddCommentsAndQuestionsAndAnswers(sourceURI, "Another users comment", "4 Another users question", "Another users answer", Guid.NewGuid(), (int)StatusName.Submitted, _context);
			//AddCommentsAndQuestionsAndAnswers(sourceURI, "Not submitted comment", "5 Not submitted question", "Not submitted answer", userId, (int)StatusName.Draft, _context);

			locationId = AddLocation("consultations://./consultation/1/document/1", _context);
			AddComment(locationId, "Just a comment", false, userId, (int)StatusName.Submitted, _context);
			locationId = AddLocation("consultations://./consultation/1/document/2/chapter/guidance", _context);
			AddComment(locationId, "Another just a comment", false, userId, (int)StatusName.Draft, _context);
			locationId = AddLocation("consultations://./consultation/1/document/2/chapter/guidance", _context);
			AddComment(locationId, "Submitted comment", false, userId, (int)StatusName.Submitted, _context);

			var questionTypeId = AddQuestionType("My Question Type", false, true, 1, _context);
			locationId = AddLocation("consultations://./consultation/1/document/2/chapter/guidance", _context);
			var questionId = AddQuestion(locationId, questionTypeId, "Question 1", _context);
			AddAnswer(questionId, userId, "This is a submitted answer", (int)StatusName.Submitted, _context);
		    AddAnswer(questionId, Guid.NewGuid(), "An answer to the same question by another user", (int)StatusName.Submitted, _context);
		    AddAnswer(questionId, userId, "This is a draft answer", (int)StatusName.Draft, _context);

			locationId = AddLocation("consultations://./consultation/1/document/2/chapter/guidance", _context);
			questionId = AddQuestion(locationId, questionTypeId, "Question 2", _context);
			//AddAnswer(questionId, userId, "This is a draft answer", (int)StatusName.Draft, _context);

			//locationId = AddLocation("consultations://./consultation/1/document/2/chapter/guidance", _context);
			//questionId = AddQuestion(locationId, questionTypeId, "Question 3", _context);
			//AddAnswer(questionId, Guid.NewGuid(), "An answer by another user", (int)StatusName.Submitted, _context);

		}
	}
}
