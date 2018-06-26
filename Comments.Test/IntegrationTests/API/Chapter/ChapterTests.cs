using Comments.Test.Infrastructure;
using NICE.Feeds.Tests.Infrastructure;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;
using TestBase = Comments.Test.Infrastructure.TestBase;

namespace Comments.Test.IntegrationTests.API.Chapter
{
    public class ChapterTests : TestBase
    {
        public ChapterTests() : base(Feed.ConsultationCommentsPublishedChapter) {}

        [Theory]
        [InlineData(-1, 1, "introduction")]
        [InlineData(0, 1, "introduction")]
        [InlineData(1, 0, "introduction")]
        [InlineData(1, -1, "introduction")]
        [InlineData(1, -1, "")]
        public void Get_Chapter_Feed_Using_Invalid_Ids_Throws_error(int consultationId, int documentId, string chapterSlug)
        {
            //Arrange
            Func<Task> response;

            // Act
            response = async () =>
            {
                await _client.GetAsync($"/consultations/api/Chapter?consultationId={consultationId}&documentId={documentId}&chapterSlug={chapterSlug}");
            };

            // Assert
            response.ShouldThrow<ArgumentException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void Get_Chapter_Feed_Using_Chapter_Slug_Throws_Error(string chapterSlug)
        {
            //Arrange
            Func<Task> response;

            // Act
            response = async () =>
            {
                await _client.GetAsync($"/consultations/api/Chapter?consultationId=1&documentId=2&chapterSlug={chapterSlug}");
            };

            // Assert
            response.ShouldThrow<ArgumentNullException>();

            //the above in a non-AAA one-liner:
            // await Assert.ThrowsAsync<ArgumentNullException>(nameof(chapterSlug), () => _client.GetAsync($"/consultations/api/Chapter?consultationId=1&documentId=2&chapterSlug={chapterSlug}"));
        }

        [Fact]
        public async Task Get_Chapter_Feed_Returns_Populated_Feed()
        {
            //Arrange (in the base constructor for this one.)

            // Act
            var response = await _client.GetAsync("/consultations/api/Chapter?consultationId=1&documentId=2&chapterSlug=introduction");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            responseString.ShouldMatchApproved();
        }

	    [Fact]
	    public async Task Get_Preview_Chapter_Feed_Returns_Populated_Feed()
	    {
		    //Arrange (in the base constructor for this one.)

		    // Act
		    var response = await _client.GetAsync("/consultations/api/Chapter?consultationId=1&documentId=1&chapterSlug=introduction&reference=GID-TA10232");
		    response.EnsureSuccessStatusCode();
		    var responseString = await response.Content.ReadAsStringAsync();

		    // Assert
		    responseString.ShouldMatchApproved();
	    }
	}
}
