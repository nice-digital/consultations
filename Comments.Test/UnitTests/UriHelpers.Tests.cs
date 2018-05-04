using Comments.Models;
using Comments.ViewModels;
using Comments.Common;
using Comments.Test.Infrastructure;
using Shouldly;
using Xunit;

namespace Comments.Test.UnitTests
{
    public class UriHelpers : TestBase
    {
        [Theory]
        [InlineData("consultations://./consultation/1/document/1/chapter/introduction", null, null, CommentOn.Chapter)]
        [InlineData("consultations://./consultation/1/document/1", null, null, CommentOn.Document)]
        [InlineData("consultations://./consultation/1", null, null, CommentOn.Consultation)]
        [InlineData("consultations://./consultation/1/document/1/chapter/introduction", null, "css path to start of range", CommentOn.Selection)]
        [InlineData("consultations://./consultation/1/document/1/chapter/introduction", "sectionHtmlElementId", null, CommentOn.Section)]
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
        public void ConvertToConsultationsUri(string relativeURL, CommentOn commentOn, string consultationsUri)
        {
            //Arrange + Act
            var uri = ConsultationsUri.ConvertToConsultationsUri(relativeURL, commentOn);

            //Assert
            uri.ShouldBe(consultationsUri);
        }

    }
}
