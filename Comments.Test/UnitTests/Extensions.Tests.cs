using Comments.Common;
using Comments.Test.Infrastructure;
using Shouldly;
using Xunit;

namespace Comments.Test.UnitTests
{
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
    }
}
