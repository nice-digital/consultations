using System;
using System.Linq;
using System.Threading.Tasks;
using Comments.Services;
using Comments.ViewModels;
using Shouldly;
using Xunit;
using TestBase = Comments.Test.Infrastructure.TestBase;

namespace Comments.Test.UnitTests
{
    public class ExportToExcelTests : TestBase
    {
	    [Fact]
	    public void Get_All_Submitted_Comments_For_URI()
	    {
			//Arrange
		    CreateALotOfData(Guid.NewGuid());
			var sourceURI = "consultations://./consultation/1";

			//Act
		    var comments = _context.GetAllSubmittedCommentsForURI(sourceURI);

			//Assert
			comments.Count.ShouldBe(2);
	    }

	    [Fact]
	    public void Get_All_Submitted_Answers_For_URI()
	    {
		    //Arrange
		    CreateALotOfData(Guid.NewGuid());
		    var sourceURI = "consultations://./consultation/1";

		    //Act
		    var answers = _context.GetAllSubmittedAnswersForURI(sourceURI);

			//Assert
		    answers.Count.ShouldBe(2);
	    }

	    [Fact]
	    public void Get_Unanswered_Questions_For_URI()
	    {
		    //Arrange
		    CreateALotOfData(Guid.NewGuid());
		    var sourceURI = "consultations://./consultation/1";

		    //Act
		    var questions = _context.GetUnansweredQuestionsForURI(sourceURI);

			//Assert
		    questions.Count.ShouldBe(1);
	    }

	    [Fact]
	    public void Get_Location_Data()
	    {
		    // Arrange
		    ResetDatabase();
		    _context.Database.EnsureCreated();
			CreateALotOfData(Guid.NewGuid());

			var sourceURI = "consultations://./consultation/1/document/1/chapter/chapter-slug";
			var comments = _context.GetAllSubmittedCommentsForURI(sourceURI);
			var exportService = new ExportService(_context, _fakeUserService, _consultationService);

			//Act
		    var locationDetails = exportService.GetLocationData(comments.First().Location);

			//Assert
			locationDetails.ConsultationName.ShouldBe("ConsultationName");
			locationDetails.DocumentName.ShouldBe("doc 1");
			locationDetails.ChapterName.ShouldBe("chapter-slug");
	    }

		private void CreateALotOfData(Guid userId)
	    {
		    int locationId, submissionId, commentId, answerId;

			locationId = AddLocation("consultations://./consultation/1", _context, "001.000.000.000");
		    commentId = AddComment(locationId, "Just a comment", false, userId, (int)StatusName.Submitted, _context);
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
		    commentId = AddComment(locationId, "Submitted comment", false, userId, (int)StatusName.Submitted, _context);
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

			locationId = AddLocation("consultations://./consultation/1/document/2", _context, "001.002.000.000");
		    questionTypeId = AddQuestionType("another Question Type", false, true, 1, _context);
			AddQuestion(locationId, questionTypeId, "Without an answer", _context);
		}
	}
}
