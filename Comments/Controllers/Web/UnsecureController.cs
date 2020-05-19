using Comments.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Comments.Controllers.Web
{
	/// <summary>
	/// This controller is just here for test purposes
	/// </summary>
	[Route("unsecure")]
	public class UnsecureController : Controller
	{
		public IActionResult Index()
		{
			var model = new SecureTestViewModel("todo: signin", "todo: signout", false, userId: null, displayName: null, givenName: null,
					surname: null, email: null, organisation: null, pageDescription: "Unsecure page");
			
			return View("../Secure/Index", model);
		}
	}
}
