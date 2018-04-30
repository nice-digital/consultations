﻿using Comments.Models;
using Comments.Test.Infrastructure;
using Shouldly;
using Xunit;

namespace Comments.Test.UnitTests
{
    public class UriHelpers : TestBase
    {
        [Theory]
        [InlineData("/1/1/introduction", null, null, "Chapter")]
        [InlineData("consultations://./consultation/1/document/1/chapter/introduction", null, null, "Chapter")]
        [InlineData("consultations://./consultation/1/document/1", null, null, "Document")]
        [InlineData("consultations://./consultation/1", null, null, "Consultation")]
        [InlineData("consultations://./consultation/1/document/1/chapter/introduction", null, "css path to start of range", "Text selection")]
        [InlineData("consultations://./consultation/1/document/1/chapter/introduction", "sectionHtmlElementId", null, "Section")]
        public void GetCommentOnParsesCorrectly(string sourceUri, string htmlElementId, string rangeStart, string commentOn)
        {
            //Arrange + Act
            var commentOnString = Common.UriHelpers.GetCommentOn(sourceUri, rangeStart, htmlElementId);

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
            var consultationsUriElements = Common.UriHelpers.ConsultationsUri.ParseConsultationsUri(sourceURI);

            //Assert
            consultationsUriElements.ConsultationId.ShouldBe(consultationId);
            consultationsUriElements.DocumentId.ShouldBe(documentId);
            consultationsUriElements.ChapterSlug.ShouldBe(chapterSlug);
        }

        [Theory]
        [InlineData("/1/1/introduction", "consultations://./consultation/1/document/1/chapter/introduction")]
        public void ConvertToConsultationsUri(string relativeURL, string consultationsUri)
        {
            //Arrange + Act
            var uri = Common.UriHelpers.ConsultationsUri.ConvertToConsultationsUri(relativeURL);

            //Assert
            uri.ShouldBe(consultationsUri);
        }

    }
}
