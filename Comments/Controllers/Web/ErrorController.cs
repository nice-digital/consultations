using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Comments.Common;
using Comments.ViewModels;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Comments.Controllers.Web
{
	/// <summary>
	/// This error controller is only shown if there's a problem showing the error using react..
	/// </summary>
	
	public class ErrorController : Controller
    {
	    private readonly ILogger _logger;
	    private readonly IHttpContextAccessor _httpContextAccessor;

	    public ErrorController(ILogger<ErrorController> logger, IHttpContextAccessor httpContextAccessor)
	    {
		    _logger = logger;
		    _httpContextAccessor = httpContextAccessor;
	    }

	    [Route(Constants.ErrorPath)]
	    [Route(Constants.ErrorPath + "/{statusCode}")]
		public ActionResult Index(int statusCode)
        {
	        var reFeat = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
	        string requestedPath = null;
	        if (reFeat != null)
	        {
		        requestedPath = reFeat.OriginalPath;
	        }

	        if (statusCode == (int)HttpStatusCode.NotFound)
	        {
		        return View("NotFound");
	        }

			var ehpFeat = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
	        Exception exception = null;
	        if (ehpFeat != null)
	        {
		        exception = ehpFeat.Error;
		        requestedPath = requestedPath ?? ehpFeat.Path;
	        }

	        _logger.LogError($"Exception for Url: {requestedPath}, Exception: {exception}");

	        var signInURL = Url.Action(Constants.Auth.LoginAction, Constants.Auth.ControllerName);
	        var signOutURL = Url.Action(Constants.Auth.LogoutAction, Constants.Auth.ControllerName);

			var errorModel = new Error(requestedPath, exception, _httpContextAccessor.HttpContext.User, signInURL, signOutURL);

			var errorCallingAPISoShouldReturnJson = (!string.IsNullOrEmpty(requestedPath) && requestedPath.StartsWith(Constants.ConsultationAPIBasePath));

			if (errorCallingAPISoShouldReturnJson)
			{
				return Json(errorModel);
			}
			return View(errorModel);
        }
    }
}
