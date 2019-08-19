using Comments.Common;
using Comments.Test.Infrastructure;
using Shouldly;
using Xunit;

namespace Comments.Test.UnitTests
{
	// All tests classes with the same Test Collection attribute
	// will not run in parallel with each other.
	[Collection("Comments.Test")]
	public class Extensions : TestBase
    {
        [Theory]
        [InlineData("/1/1/introduction", "/consultations/1/1/introduction")]
        [InlineData("/1/1/InTrOdUcTiOn", "/consultations/1/1/introduction")]
        [InlineData("/consultations/1/1/introduction", "/consultations/1/1/introduction")]
        [InlineData("/consultations/1/1/InTrOdUcTiOn", "/consultations/1/1/introduction")]
        [InlineData("1/1/InTrOdUcTiOn", "/consultations/1/1/introduction")]
        public void ToConsultationsRelativeUrlReturnsValidResults(string relativeUrl, string expectedConsultationsRelativeUrl)
        {
            //Arrange + Act
            var actualConsultationsRelativeUrl = relativeUrl.ToConsultationsRelativeUrl();

            //Assert
            actualConsultationsRelativeUrl.ShouldBe(expectedConsultationsRelativeUrl);
        }

        [Theory]
        [InlineData("http://www.nice.org.uk/1/1/introduction", "https://www.nice.org.uk/1/1/introduction")]
        [InlineData("hTtP://www.nice.org.uk/1/1/introduction", "https://www.nice.org.uk/1/1/introduction")]
        [InlineData("https://www.nice.org.uk/1/1/introduction", "https://www.nice.org.uk/1/1/introduction")]
        [InlineData("hTtPs://www.nice.org.uk/1/1/introduction", "hTtPs://www.nice.org.uk/1/1/introduction")]
        public void ToHTTPSReturnsValidResults(string urlIn, string expectedUrl)
        {
            //Arrange + Act
            var actualUrl = urlIn.ToHTTPS();

            //Assert
            actualUrl.ShouldBe(expectedUrl);
        }
    }
}
