using Comments.Test.Infrastructure;
using Shouldly;
using System;
using System.Net;
using System.Threading.Tasks;
using NICE.Feeds.Tests.Infrastructure;
using Xunit;
using TestBase = Comments.Test.Infrastructure.TestBase;

namespace Comments.Test.IntegrationTests.API.Documents
{
	// All tests classes with the same Test Collection attribute
	// will not run in parallel with each other.
	[Collection("Comments.Test")]
	public class DocumentsTests : TestBase
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Get_Documents_Feed_Using_Invalid_Consultation_Id_Throws_error(int consultationId)
        {
	        //Arrange (in the base constructor for this one.)

	        //Act
	        var response = await _client.GetAsync($"/consultations/api/Documents?consultationId={consultationId}");
			var responseString = await response.Content.ReadAsStringAsync();

	        //Assert
	        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
	        responseString.ShouldMatchApproved(new Func<string, string>[] { Scrubbers.ScrubErrorMessage });
		}

        [Fact]
        public async Task Get_Documents_Feed_Returns_Populated_Feed()
        {
            //Arrange (in base constructor)

            // Act
            var response = await _client.GetAsync("/consultations/api/Documents?consultationId=54");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            responseString.ShouldMatchApproved();
        }

        [Fact]
        public async Task Get_Documents_Feed_Fills_In_Blank_Document_Titles()
        {
            //Arrange (in base constructor)
            FeedToUse = Feed.ConsultationCommentsPublishedDetailNoTitle;

            // Act
            var response = await _client.GetAsync("/consultations/api/Documents?consultationId=54");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            responseString.ShouldMatchApproved();
        }

	    [Fact]
	    public async Task Get_Draft_Preview_Documents_Feed_Returns_Populated_Feed()
	    {
		    //Arrange (in base constructor)

		    // Act
		    var response = await _client.GetAsync("/consultations/api/PreviewDraftDocuments?consultationId=113&documentId=1&reference=GID-NG10186");
		    response.EnsureSuccessStatusCode();
		    var responseString = await response.Content.ReadAsStringAsync();

		    // Assert
		    responseString.ShouldMatchApproved();
	    }

	    [Fact]
	    public async Task Get_Published_Preview_Documents_Feed_Returns_Populated_Feed()
	    {
		    //Arrange (in base constructor)

		    // Act
		    var response = await _client.GetAsync("/consultations/api/PreviewPublishedDocuments?consultationId=113&documentId=1");
		    response.EnsureSuccessStatusCode();
		    var responseString = await response.Content.ReadAsStringAsync();

		    // Assert
		    responseString.ShouldMatchApproved();
	    }
	}
}
