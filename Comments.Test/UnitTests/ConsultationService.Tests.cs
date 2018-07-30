using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Linq;
using System.Net;
using NICE.Feeds;
using NICE.Feeds.Models.Indev.Detail;
using NICE.Feeds.Tests.Infrastructure;
using Xunit;

namespace Comments.Test.UnitTests
{
	public class ConsultationServiceTests : Comments.Test.Infrastructure.TestBase
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

	        var context = new ConsultationsContext(_options, userService, _fakeEncryption);
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

	    [Theory]
		//document page tests
		[InlineData(null, "gid-ng10107", "html-content", "/guidance/indevelopment/gid-ng10107/consultation/html-content", null)] //regular indev project
	    [InlineData("", "gid-ng10107", "html-content", "/guidance/indevelopment/gid-ng10107/consultation/html-content", null)]
	    [InlineData("orig-ref", "gid-ng10107", "html-content", "/guidance/orig-ref/update/gid-ng10107/consultation/html-content", null)] //an "update project"
	    [InlineData(null, "ph24", "html-content", "/guidance/ph24/consultation/html-content", null)] //published product
		public void GetBreadcrumbForDocumentPage(string origProjectReference, string reference, string resourceTitleId, string expectedConsultationUrl, string expectedDocumentsUrl)
	    {
			//Arrange
		    var consultationService = new Services.ConsultationService(null, null, null, null);
		    var consultationTitle = "Some consultation title";
			var consultationDetail = new ConsultationDetail()
		    {
			    OrigProjectReference = origProjectReference,
			    Reference = reference,
			    ResourceTitleId = resourceTitleId,
				Title = consultationTitle
			};

			//Act
		    var actualBreadcrumb = consultationService.GetBreadcrumbs(consultationDetail, false);

			//Assert
			actualBreadcrumb.ShouldNotBeNull();
		    actualBreadcrumb.Count().ShouldBe(2);

		    actualBreadcrumb.First().Label.ShouldBe("Home");
		    actualBreadcrumb.First().Url.ShouldBe("/");

		    actualBreadcrumb.Skip(1).First().Label.ShouldBe(consultationTitle);
		    actualBreadcrumb.Skip(1).First().Url.ShouldBe(expectedConsultationUrl);
		}

		[Theory]
		[InlineData("/guidance/indevelopment/gid-ta10232/consultation/html-content", "/consultations/1/2/introduction")] //regular indev project
		[InlineData("/guidance/orig-ref/update/gid-ta10232/consultation/html-content", "/consultations/1/2/introduction")] //an "update project"
		public void GetBreadcrumbForReviewPage(string expectedConsultationUrl, string expectedDocumentsUrl)
		{
			//Arrange
			var feedService = new FeedService(new FeedReader(Feed.ConsultationCommentsPublishedDetailMulitpleDoc));
			var consultationService = new Services.ConsultationService(null, feedService, null, null);
			var consultationDetail = feedService.GetIndevConsultationDetailForPublishedProject(1, PreviewState.NonPreview, 2);

			//Act
			var actualBreadcrumb = consultationService.GetBreadcrumbs(consultationDetail, true);

			//Assert
			actualBreadcrumb.ShouldNotBeNull();
			actualBreadcrumb.Count().ShouldBe(3);

			actualBreadcrumb.First().Label.ShouldBe("Home");
			actualBreadcrumb.First().Url.ShouldBe("/");

			actualBreadcrumb.Skip(1).First().Label.ShouldBe("For consultation comments");
			//actualBreadcrumb.Skip(1).First().Url.ShouldBe(expectedConsultationUrl); //TODO: uncomment this, when the indev feed is fixed in ID-215

			actualBreadcrumb.Skip(2).First().Label.ShouldBe("Consultation documents");
			actualBreadcrumb.Skip(2).First().Url.ShouldBe(expectedDocumentsUrl);
		}
	}
}
