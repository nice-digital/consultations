using Shouldly;
using Xunit;

namespace Comments.Test.UnitTests
{
	public class ReviewPageTests
	{

		[Theory]
		[InlineData("", "0")]
		[InlineData(null, "0")]
		[InlineData("1", "1")]
		[InlineData("1.1", "1.1")]
		[InlineData("1.10", "01.10")]
		[InlineData("2.1.0.0.2.1.1.0.0.1.2.167", "002.001.000.000.002.001.001.000.000.001.002.167")]

		public void OrderingSetAccessorInViewModelPadsZeros(string orderStringPassedIn, string expectedOrderString)
		{
			//Arrange 
			var location = new ViewModels.Location();

			//Act
			location.Order = orderStringPassedIn;

			//Assert
			location.Order.ShouldBe(expectedOrderString);
		}
	}
}
