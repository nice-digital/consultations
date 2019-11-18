using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace Comments.Test.Infrastructure
{
	public static class FakeLinkGenerator
	{
		public static LinkGenerator Get()
		{
			
			var linkGenerator = new Mock<LinkGenerator>();

			linkGenerator.Setup(g => g.GetPathByAddress<RouteValuesAddress>(It.IsAny<HttpContext>(),
						It.IsAny<RouteValuesAddress>(), It.IsAny<RouteValueDictionary>(),
						It.IsAny<RouteValueDictionary>(), It.IsAny<PathString?>(),
						It.IsAny<FragmentString>(), It.IsAny<LinkOptions>()))
						.Returns("/");

				//.Setup(g => g.GetPathByAddress(
				//	It.IsAny<httpcontext>(),
				//	It.IsAny<routevaluesaddress>(),
				//	It.IsAny<routevaluedictionary>(),
				//	It.IsAny<routevaluedictionary>(),
				//	It.IsAny<pathstring?>(),
				//	It.IsAny<fragmentstring>(),
				//	It.IsAny<linkoptions>()))
				//.Returns("/");

			return linkGenerator.Object;
		}
	}
}
