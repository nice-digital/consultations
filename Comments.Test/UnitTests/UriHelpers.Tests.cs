using Comments.Test.Infrastructure;
using Shouldly;
using Xunit;

namespace Comments.Test.UnitTests
{
    public class UriHelpers : TestBase
    {
        [Theory]
        [InlineData("/1/1/introduction", "Chapter")]
        [InlineData("consultations://./consultation/1/document/1/chapter/introduction", "Chapter")]
        [InlineData("consultations://./consultation/1/document/1", "Document")]
        [InlineData("consultations://./consultation/1", "Consultation")]  //todo: more cases
        public void GetCommentOnParsesCorrectly(string url, string commentOn)
        {
            //Arrange, Act and Assert
            Common.UriHelpers.GetCommentOn(url).ShouldBe(commentOn);
        }

        [Theory]
        [InlineData("consultations://./consultation/1/document/1/chapter/introduction", 1, 1, "introduction")] //todo: more cases
        public void ParseConsultationsUri(string sourceURI, int consultationId, int? documentId, string chapterSlug)
        {
            //Arrange + Act 
            var consultationsUriElements = Common.UriHelpers.ConsultationsUri.ParseConsultationsUri(sourceURI);

            //Assert
            consultationsUriElements.ConsultationId.ShouldBe(1);
            consultationsUriElements.DocumentId.ShouldBe(1);
            consultationsUriElements.ChapterSlug.ShouldBe("introduction");
        }

    }
}
