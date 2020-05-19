using Comments.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Comments.Common;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using NICE.Identity.Authentication.Sdk.Extensions;

namespace Comments.Controllers.Web
{
	/// <summary>
	/// This controller is just here for test purposes
	/// </summary>
	[Route("secure")]
	[Authorize]
	public class SecureController : Controller
	{
		private readonly ILogger<RootController> _logger;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public SecureController(ILogger<RootController> logger, IHttpContextAccessor httpContextAccessor)
		{
			_logger = logger;
			_httpContextAccessor = httpContextAccessor;
		}

		public IActionResult Index()
		{
			_logger.LogWarning("Hitting the secure controller. this should only occur locally.");
			var pageDescription = "Hitting the secure controller";

			var user = _httpContextAccessor.HttpContext.User;
			var isLoggedOn = (user != null && user.Identity.IsAuthenticated);
			// var xReferer = Request.GetUri().GetComponents(UriComponents.Scheme | UriComponents.Host, UriFormat.UriEscaped);
			var signInUrl = Url.Action(Constants.Auth.LoginAction, Constants.Auth.ControllerName, new {returnUrl = Request.GetUri().PathAndQuery });
			var signOutUrl = Url.Action(Constants.Auth.LogoutAction, Constants.Auth.ControllerName, new { returnUrl = Request.GetUri().PathAndQuery});

			SecureTestViewModel model;

			if (isLoggedOn)
			{

				model = new SecureTestViewModel(signInUrl, signOutUrl, isLoggedOn,
					  userId: user.NameIdentifier(), displayName: user.DisplayName(), givenName: user.FirstName(), surname: user.LastName(),
					  email: "todo: email", organisation: "todo: organisation", pageDescription: pageDescription);
			}
			else
			{
				model = new SecureTestViewModel(signInUrl, signOutUrl, isLoggedOn, userId: null, displayName: null, givenName: null,
					surname: null, email: null, organisation: null, pageDescription: pageDescription);
			}

			return View(model);
		}
	}
}
