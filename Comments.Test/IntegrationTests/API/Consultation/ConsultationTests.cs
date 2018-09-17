using System;
using System.Net;
using System.Threading.Tasks;
using Comments.Test.Infrastructure;
using NICE.Feeds.Tests.Infrastructure;
using Shouldly;
using Xunit;
using TestBase = Comments.Test.Infrastructure.TestBase;

namespace Comments.Test.IntegrationTests.API.Consultation
{
    public class ConsultationTests : TestBase
    {
        public ConsultationTests() : base(Feed.ConsultationCommentsPublishedDetailMulitpleDoc, true, Guid.Empty, "Benjamin Button") { }
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Get_Consultation_Feed_Using_Invalid_Consultation_Id_Throws_error(int consultationId)
        {
	        //Act
	        var response = await _client.GetAsync($"/consultations/api/Consultation?consultationId={consultationId}");
			var responseString = await response.Content.ReadAsStringAsync();

	        //Assert
	        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
	        responseString.ShouldMatchApproved(new Func<string, string>[] { Scrubbers.ScrubErrorMessage });
		}

        [Fact]
        public async Task Get_Consultation_Feed_Returns_Populated_Feed()
        {
            //Arrange (in base constructor)

            // Act
            var response = await _client.GetAsync("/consultations/api/Consultation?consultationId=1");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            responseString.ShouldMatchApproved();
        }

	    [Fact]
	    public async Task Get_Draft_Consultation_Feed_Returns_Populated_Feed()
	    {
		    //Arrange (in base constructor)

		    // Act
		    var response = await _client.GetAsync("/consultations/api/DraftConsultation?consultationId=113&documentId=1&reference=GID-NG10186");
		    response.EnsureSuccessStatusCode();
		    var responseString = await response.Content.ReadAsStringAsync();

		    // Assert
		    responseString.ShouldMatchApproved();
	    }
	}
}
