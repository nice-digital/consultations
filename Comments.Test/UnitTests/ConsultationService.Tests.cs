using Comments.Models;
using Comments.Services;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NICE.Feeds;
using NICE.Feeds.Indev;
using NICE.Feeds.Indev.Models;
using NICE.Feeds.Indev.Models.Detail;
using NICE.Feeds.Tests.Infrastructure;
using Xunit;

namespace Comments.Test.UnitTests
{
	public class ConsultationServiceTests : Comments.Test.Infrastructure.TestBase
    {
        [Fact]
        public async Task Comments_CanBeRead()
        { 
            // Arrange
            ResetDatabase();
	        _context.Database.EnsureCreated();

			var sourceURI = "consultations://./consultation/1/document/1/chapter/introduction";
            var commentText = Guid.NewGuid().ToString();
            var createdByUserId = Guid.NewGuid().ToString();

            var userService = FakeUserService.Get(true, "Benjamin Button", createdByUserId);

            var locationId = AddLocation(sourceURI);
            AddComment(locationId, commentText, createdByUserId: createdByUserId);

	        var context = new ConsultationsContext(_options, userService, _fakeEncryption);
            var commentService = new CommentService(context, userService, _consultationService, _fakeHttpContextAccessor);
            
            // Act
            var viewModel = await commentService.GetCommentsAndQuestions(sourceURI, _urlHelper);

            //Assert
            viewModel.Comments.Single().CommentText.ShouldBe(commentText);
        }

        

	    [Theory]
		//document page tests
		[InlineData(null, "gid-ng10107", "html-content", "/guidance/indevelopment/gid-ng10107/consultation/html-content", null)] //regular indev project
	    [InlineData("", "gid-ng10107", "html-content", "/guidance/indevelopment/gid-ng10107/consultation/html-content", null)]
	    [InlineData("orig-ref", "gid-ng10107", "html-content", "/guidance/orig-ref/update/gid-ng10107/consultation/html-content", null)] //an "update project"
	    [InlineData(null, "ph24", "html-content", "/guidance/ph24/consultation/html-content", null)] //published product
		public async Task GetBreadcrumbForDocumentPage(string origProjectReference, string reference, string resourceTitleId, string expectedConsultationUrl, string expectedDocumentsUrl)
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
		    var actualBreadcrumb = await consultationService.GetBreadcrumbs(consultationDetail, BreadcrumbType.DocumentPage);

			//Assert
			actualBreadcrumb.ShouldNotBeNull();
		    actualBreadcrumb.Count().ShouldBe(4);

		    actualBreadcrumb.First().Label.ShouldBe("Home");
		    actualBreadcrumb.First().Url.ShouldBe("/");

		    actualBreadcrumb.Skip(1).First().Label.ShouldBe("All consultations");
		    actualBreadcrumb.Skip(1).First().Url.ShouldBe("/guidance/inconsultation");
		    actualBreadcrumb.Skip(1).First().LocalRoute.ShouldBeFalse();

			actualBreadcrumb.Skip(2).First().Label.ShouldBe("Consultation responses");
		    actualBreadcrumb.Skip(2).First().Url.ShouldBe("/");
		    actualBreadcrumb.Skip(2).First().LocalRoute.ShouldBeTrue();

			actualBreadcrumb.Skip(3).First().Label.ShouldBe(consultationTitle);
		    actualBreadcrumb.Skip(3).First().Url.ShouldBe(expectedConsultationUrl);
		}

		[Theory]
		[InlineData("/guidance/indevelopment/gid-ta10232/consultation/html-content", "/1/2/introduction")] //regular indev project
		[InlineData("/guidance/orig-ref/update/gid-ta10232/consultation/html-content", "/1/2/introduction")] //an "update project"
		public async Task GetBreadcrumbForReviewPage(string expectedConsultationUrl, string expectedDocumentsUrl)
		{
			//Arrange
			var feedService = new IndevFeedService(new FeedReader(Feed.ConsultationCommentsPublishedDetailMulitpleDoc));
			var consultationService = new Services.ConsultationService(null, feedService, null, null);
			var consultationDetail = await feedService.GetIndevConsultationDetailForPublishedProject(1, PreviewState.NonPreview, 2);

			//Act
			var actualBreadcrumb = await consultationService.GetBreadcrumbs(consultationDetail, BreadcrumbType.Review); 

			//Assert
			actualBreadcrumb.ShouldNotBeNull();
			actualBreadcrumb.Count().ShouldBe(5);

			actualBreadcrumb.First().Label.ShouldBe("Home");
			actualBreadcrumb.First().Url.ShouldBe("/");

			actualBreadcrumb.Skip(1).First().Label.ShouldBe("All consultations");
			actualBreadcrumb.Skip(1).First().Url.ShouldBe("/guidance/inconsultation");
			actualBreadcrumb.Skip(1).First().LocalRoute.ShouldBeFalse();

			actualBreadcrumb.Skip(2).First().Label.ShouldBe("Consultation responses");
			actualBreadcrumb.Skip(2).First().Url.ShouldBe("/");
			actualBreadcrumb.Skip(2).First().LocalRoute.ShouldBeTrue();

			actualBreadcrumb.Skip(3).First().Label.ShouldBe("For consultation comments");
			//actualBreadcrumb.Skip(3).First().Url.ShouldBe(expectedConsultationUrl); //TODO: uncomment this, when the indev feed is fixed in ID-215

			actualBreadcrumb.Skip(4).First().Label.ShouldBe("Consultation documents");
			actualBreadcrumb.Skip(4).First().Url.ShouldBe(expectedDocumentsUrl);
		}

	    [Fact]
	    public async Task GetBreadcrumbsReturnsNullForBreadcrumbTypeOfNone()
	    {
			//Arrange
		    var feedService = new IndevFeedService(new FeedReader(Feed.ConsultationCommentsPublishedDetailMulitpleDoc));
			var consultationService = new Services.ConsultationService(null, feedService, null, null);
		    var consultationDetail = await feedService.GetIndevConsultationDetailForPublishedProject(1, PreviewState.NonPreview, 2);

			//Act
		    var breadcrumbs = await consultationService.GetBreadcrumbs(consultationDetail, BreadcrumbType.None);

		    //Act
			breadcrumbs.ShouldBeNull();
	    }
	}
}
