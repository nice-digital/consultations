using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Comments.Services;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Newtonsoft.Json;
using NICE.Feeds.Tests.Infrastructure;
using Shouldly;
using Xunit;
using TestBase = Comments.Test.Infrastructure.TestBase;

namespace Comments.Test.IntegrationTests.API.Consultation
{
    public class ConsultationTests : TestBase
    {
        public ConsultationTests() : base(Feed.ConsultationCommentsPublishedDetailMulitpleDoc, true, Guid.Empty, "Benjamin Button") { }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Get_Consultation_Feed_Using_Invalid_Consultation_Id_Throws_error(int consultationId)
        {
	        //Act
	        var response = await _client.GetAsync($"/consultations/api/Consultation?consultationId={consultationId}");
			var responseString = await response.Content.ReadAsStringAsync();

	        //Assert
	        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
	        responseString.ShouldMatchApproved(new Func<string, string>[] { Scrubbers.ScrubErrorMessage });
		}

        [Fact]
        public async Task Get_Consultation_Feed_Returns_Populated_Feed()
        {
            //Arrange (in base constructor)

            // Act
            var response = await _client.GetAsync("/consultations/api/Consultation?consultationId=1");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            responseString.ShouldMatchApproved();
        }

	    [Fact]
	    public async Task Get_Draft_Consultation_Feed_Returns_Populated_Feed()
	    {
		    //Arrange (in base constructor)

		    // Act
		    var response = await _client.GetAsync("/consultations/api/DraftConsultation?consultationId=113&documentId=1&reference=GID-NG10186");
		    response.EnsureSuccessStatusCode();
		    var responseString = await response.Content.ReadAsStringAsync();

		    // Assert
		    responseString.ShouldMatchApproved();
	    }

	    [Fact]
	    public async void CommentsQuestionsAndAnswers_CanBeRead()
	    {
		    // Arrange
		    ResetDatabase();
			_context.Database.EnsureCreated();

			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
		    var commentText = Guid.NewGuid().ToString();
		    var questionText = Guid.NewGuid().ToString();
		    var answerText = Guid.NewGuid().ToString();
		    var createdByUserId = Guid.Empty;
		    var authenticateService = new FakeAuthenticateService(authenticated: true);

		    AddCommentsAndQuestionsAndAnswers(sourceURI, commentText, questionText, answerText, createdByUserId, (int)StatusName.Draft, _context);
		    //var submitService = new SubmitService(_context, _fakeUserService, _consultationService);
		    var commentService = new CommentService(_context, _fakeUserService, authenticateService, _consultationService);

		    // Act    
		    var viewModel = commentService.GetCommentsAndQuestions(sourceURI);

		    //Assert
		    viewModel.Comments.Single().CommentText.ShouldBe(commentText);
		    var question = viewModel.Questions.Single();
		    question.QuestionText.ShouldBe(questionText);
		    question.Answers.Single().AnswerText.ShouldBe(answerText);


		    //sample code:

		    var resp = await _client.GetAsync($"/consultations/api/Comments?sourceURI={WebUtility.UrlEncode(sourceURI)}");
		    resp.EnsureSuccessStatusCode();
		    var responseString = await resp.Content.ReadAsStringAsync();
		    var anotherQuestion = JsonConvert.DeserializeObject<CommentsAndQuestions>(responseString);
		    anotherQuestion.Questions.Single().Answers.Single().AnswerText.ShouldBe(answerText);

	    }
	}
}
