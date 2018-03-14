using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Comments.Test.Infrastructure;
using Comments.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using Xunit;

namespace Comments.Test.IntegrationTests.API.Chapter
{
    public class ChapterTests : TestBase
    {
        [Theory]
        [InlineData(-1, 1, "introduction")]
        [InlineData(0, 1, "introduction")]
        [InlineData(1, 0, "introduction")]
        [InlineData(1, -1, "introduction")]
        [InlineData(1, -1, "")]
        public async Task Get_Chapter_Feed_Using_Invalid_Ids_Returns_Empty_Feed(int consultationId, int documentId, string chapterSlug)
        {
            //Arrange (in the base constructor for this one.)

            // Act
            var response = await _client.GetAsync($"/consultations/api/Chapter?consultationId={consultationId}&documentId={documentId}&chapterSlug={chapterSlug}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound); //maybe should be a 500?
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task Get_Chapter_Feed_Using_Chapter_Slug_Throws_Error(string chapterSlug)
        {
            //Arrange (in the base constructor for this one.)

            // Act
            var response = await _client.GetAsync($"/consultations/api/Chapter?consultationId=1&documentId=2&chapterSlug={chapterSlug}");
           // response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            //var deserialisedResponse = JsonConvert.DeserializeObject<JObject>(responseString);
            //deserialisedResponse.
            responseString.ShouldMatchApproved();
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
    }
}
