using Comments.Test.Infrastructure;
using Shouldly;
using Xunit;

namespace Comments.Test.UnitTests
{
    public class UriHelpers : TestBase
    {
        [Theory]
        [InlineData("/1/1/introduction", "Chapter")]
        public void GetCommentOnParsesCorrectly(string url, string commentOn)
        {
            //Arrange, Act and Assert
            Common.UriHelpers.GetCommentOn(url).ShouldBe(commentOn);
        }

    }
}
