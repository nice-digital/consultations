using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Linq;
using System.Net;
using Xunit;

namespace Comments.Test.UnitTests
{
	public class Tests : Comments.Test.Infrastructure.TestBase
    {
        [Fact]
        public void Comments_CanBeRead()
        { 
            // Arrange
            ResetDatabase();
	        _context.Database.EnsureCreated();

			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
            var commentText = Guid.NewGuid().ToString();
            var createdByUserId = Guid.NewGuid();

            var userService = FakeUserService.Get(true, "Benjamin Button", createdByUserId);
            var authenticateService = new FakeAuthenticateService(authenticated: true);

            var locationId = AddLocation(sourceURI);
            AddComment(locationId, commentText, isDeleted: false, createdByUserId: createdByUserId);

	        var context = new ConsultationsContext(_options, userService);
			//var submitService = new SubmitService(context, userService, _consultationService);
            var commentService = new CommentService(context, userService, authenticateService, _consultationService);
            
            // Act
            var viewModel = commentService.GetCommentsAndQuestions(sourceURI);

            //Assert
            viewModel.Comments.Single().CommentText.ShouldBe(commentText);
        }

        [Fact]
        public async void CommentsQuestionsAndAnswers_CanBeRead()
        {
            // Arrange
            ResetDatabase();
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

	    [Fact]
	    public void GetBreadcrumbForDocumentPage()
	    {
		    //Arrange
			var consultationService = new ConsultationService(null, null, null, null);

			//Act
		    var actualBreadcrumb = consultationService.GetBreadcrumb(1, false);


			//Assert
			actualBreadcrumb.Links.ShouldNotBeNull();
		    actualBreadcrumb.Links.Count().ShouldBe(2);

			actualBreadcrumb.Links.First().Text.ShouldBe("All consultations");
		    actualBreadcrumb.Links.First().Url.ShouldBe("/guidance/inconsultation");

		    actualBreadcrumb.Links.Skip(1).First().Text.ShouldBe("Consultation");
		    actualBreadcrumb.Links.Skip(1).First().Url.ShouldBe("/guidance/indevelopment/gid-ng10107/consultation/html-content");
		}

	}
}
