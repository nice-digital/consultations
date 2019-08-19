using Comments.Models;
using Comments.ViewModels;
using Comments.Common;
using Comments.Test.Infrastructure;
using Shouldly;
using Xunit;

namespace Comments.Test.UnitTests
{
	// All tests classes with the same Test Collection attribute
	// will not run in parallel with each other.
	[Collection("Comments.Test")]
	public class UriHelpers : TestBase
    {
        [Theory]
        [InlineData("consultations://./consultation/1/document/1/chapter/introduction", null, null, CommentOn.Chapter)]
        [InlineData("consultations://./consultation/1/document/1", null, null, CommentOn.Document)]
        [InlineData("consultations://./consultation/1", null, null, CommentOn.Consultation)]
        [InlineData("consultations://./consultation/1/document/1/chapter/introduction", null, "css path to start of range", CommentOn.Selection)]
        [InlineData("consultations://./consultation/1/document/1/chapter/introduction", "sectionHtmlElementId", null, CommentOn.Section)]
        [InlineData("consultations://./consultation/1/document/1/chapter/introduction", "np-1.1.1", null, CommentOn.SubSection)]
		public void GetCommentOnParsesCorrectly(string sourceUri, string htmlElementId, string rangeStart, CommentOn commentOn)
        {
            //Arrange + Act
            var commentOnString = CommentOnHelpers.GetCommentOn(sourceUri, rangeStart, htmlElementId);

            //Assert
            commentOnString.ShouldBe(commentOn);
        }

        [Theory]
        [InlineData("consultations://./consultation/1/document/1/chapter/introduction", 1, 1, "introduction")]
        [InlineData("consultations://./consultation/1/document/1", 1, 1, null)]
        [InlineData("consultations://./consultation/1", 1, null, null)]
        public void ParseConsultationsUri(string sourceURI, int consultationId, int? documentId, string chapterSlug)
        {
            //Arrange + Act 
            var consultationsUriElements = ConsultationsUri.ParseConsultationsUri(sourceURI);

            //Assert
            consultationsUriElements.ConsultationId.ShouldBe(consultationId);
            consultationsUriElements.DocumentId.ShouldBe(documentId);
            consultationsUriElements.ChapterSlug.ShouldBe(chapterSlug);
        }

        [Theory]
        [InlineData("consultations://./consultation/1/document/1/chapter/introduction", CommentOn.Consultation, "consultations://./consultation/1")]
        [InlineData("consultations://./consultation/1/document/1/chapter/introduction", CommentOn.Document, "consultations://./consultation/1/document/1")]
        [InlineData("consultations://./consultation/1/document/1/chapter/introduction", CommentOn.Chapter, "consultations://./consultation/1/document/1/chapter/introduction")]
        [InlineData("consultations://./consultation/1/document/1/chapter/introduction", CommentOn.Section, "consultations://./consultation/1/document/1/chapter/introduction")]
        [InlineData("consultations://./consultation/1/document/1/chapter/introduction", CommentOn.Selection, "consultations://./consultation/1/document/1/chapter/introduction")]
        [InlineData("consultations://./consultation/1/document/1/chapter/introduction", CommentOn.SubSection, "consultations://./consultation/1/document/1/chapter/introduction")]
		public void ConvertToConsultationsUri(string relativeURL, CommentOn commentOn, string consultationsUri)
        {
            //Arrange + Act
            var uri = ConsultationsUri.ConvertToConsultationsUri(relativeURL, commentOn);

            //Assert
            uri.ShouldBe(consultationsUri);
        }

	    [Fact]
	    public void CreateConsultationsURI()
	    {
		    //Arrange + Act
		    var consultationId = 1;
		    var uri = ConsultationsUri.CreateConsultationURI(consultationId);

		    //Assert
		    uri.ShouldBe("consultations://./consultation/1");
		}

	    [Fact]
	    public void CreateDocumentURI()
	    {
		    //Arrange + Act
		    var uri = ConsultationsUri.CreateDocumentURI(10, 100);

		    //Assert
		    uri.ShouldBe("consultations://./consultation/10/document/100");
	    }

	    [Theory]
		[InlineData("/1/review", true)]
	    [InlineData("/1/REVIEW", true)]
	    [InlineData("/1/Review", true)]
	    [InlineData("/999/Review", true)]
	    [InlineData("/1/1/introduction", false)]
		public void IsReviewPageRelativeUrlParsesCorrectly(string relativeURL, bool expectedOutput)
	    {
			//Arrange + Act
		    var actualOutput = ConsultationsUri.IsReviewPageRelativeUrl(relativeURL);

		    //Assert
		    actualOutput.ShouldBe(expectedOutput);
		}

	    [Theory]
	    [InlineData(null, false)]
	    [InlineData("", false)]
		[InlineData("invalid uri", false)]
	    [InlineData("/1/1/introduction", false)]
	    [InlineData("http://www.nice.org.uk/consultations/1/1/introduction", false)]
	    [InlineData("consultations://./consultation/", false)]
	    [InlineData("consultations://./consultation/1", true)]
	    [InlineData("consultations://./consultation/1/document/1", true)]
	    [InlineData("consultations://./consultation/1/document/1/chapter/introduction", true)]
		public void IsValidSourceURIParsesCorrectly(string uri, bool expectedOutput)
	    {
		    //Arrange + Act
		    var actualOutput = ConsultationsUri.IsValidSourceURI(uri);

		    //Assert
		    actualOutput.ShouldBe(expectedOutput);
		}

	}
}
