using System;
using System.Threading.Tasks;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Shouldly;
using Xunit;

namespace Comments.Test.IntegrationTests.API.Export
{
    public class ExportTests :  TestBase
    {
	    [Fact]
	    public async Task Create_Spreadsheet()
	    {
		    // Arrange
		    ResetDatabase();
		    _context.Database.EnsureCreated();
		    var userId = Guid.NewGuid();
		    CreateALotOfData(userId);
		    var consultationId = 1;

		    // Act
		    var response = await _client.GetAsync($"consultations/api/ExportExternal/{consultationId}");
		    response.EnsureSuccessStatusCode();

		    //Assert
		    response.IsSuccessStatusCode.ShouldBeTrue();
	    }

		private void CreateALotOfData(Guid userId)
		{
			int locationId, submissionId, commentId, answerId;

			locationId = AddLocation("consultations://./consultation/1", _context, "001.000.000.000");
			commentId = AddComment(locationId, "Submitted comment", false, userId, (int)StatusName.Submitted, _context);
			submissionId = AddSubmission(userId, _context);
			AddSubmissionComments(submissionId, commentId, _context);

			locationId = AddLocation("consultations://./consultation/1", _context, "001.000.000.000");
			commentId = AddComment(locationId, "Deleted comment", true, userId, (int)StatusName.Submitted, _context);
			submissionId = AddSubmission(userId, _context);
			AddSubmissionComments(submissionId, commentId, _context);

			locationId = AddLocation("consultations://./consultation/1/document/2/chapter/introduction", _context, "001.002.002.000");
			commentId = AddComment(locationId, "Draft comment", false, userId, (int)StatusName.Draft, _context);
			AddSubmissionComments(submissionId, commentId, _context);

			locationId = AddLocation("consultations://./consultation/1/document/1/chapter/chapter-slug", _context, "001.002.000.000");
			commentId = AddComment(locationId, "Another Users Submitted comment", false, Guid.NewGuid(), (int)StatusName.Submitted, _context);
			AddSubmissionComments(submissionId, commentId, _context);

			var questionTypeId = AddQuestionType("My Question Type", false, true, 1, _context);
			locationId = AddLocation("consultations://./consultation/1/document/2/chapter/guidance", _context, "001.002.001.000");
			var questionId = AddQuestion(locationId, questionTypeId, "Question 1", _context);
			answerId = AddAnswer(questionId, userId, "This is a submitted answer", (int)StatusName.Submitted, _context);
			submissionId = AddSubmission(userId, _context);
			AddSubmissionAnswers(submissionId, answerId, _context);
			answerId = AddAnswer(questionId, Guid.NewGuid(), "An answer to the same question by another user", (int)StatusName.Submitted, _context);
			AddSubmissionAnswers(submissionId, answerId, _context);
			answerId = AddAnswer(questionId, userId, "This is a draft answer", (int)StatusName.Draft, _context);
			AddSubmissionAnswers(submissionId, answerId, _context);

			questionTypeId = AddQuestionType("Question Type", false, true, 1, _context);
			locationId = AddLocation("consultations://./consultation/1/document/2/chapter/guidance", _context, "001.002.001.000");
			questionId = AddQuestion(locationId, questionTypeId, "Question 2", _context);
			answerId = AddAnswer(questionId, Guid.NewGuid(), "Another Users answer", (int)StatusName.Draft, _context);
			AddSubmissionAnswers(submissionId, answerId, _context);

			locationId = AddLocation("consultations://./consultation/1/document/2", _context, "001.002.000.000");
			questionTypeId = AddQuestionType("another Question Type", false, true, 1, _context);
			AddQuestion(locationId, questionTypeId, "Without an answer", _context);
		}
	}
}
