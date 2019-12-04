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
	    private readonly LinkGenerator _linkGenerator;

	    public ErrorController(ILogger<ErrorController> logger, IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
	    {
		    _logger = logger;
		    _httpContextAccessor = httpContextAccessor;
		    _linkGenerator = linkGenerator;
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

			var ehFeat = HttpContext.Features.Get<IExceptionHandlerFeature>();
	        Exception exception = null;
	        if (ehFeat != null)
	        {
		        exception = ehFeat.Error;
	        }

	        _logger.LogError($"Exception for Url: {requestedPath}, Exception: {exception}");

	        var signInURL = _linkGenerator.GetPathByAction(Constants.Auth.LoginAction, Constants.Auth.ControllerName);
	        var signOutURL = _linkGenerator.GetPathByAction(Constants.Auth.LogoutAction, Constants.Auth.ControllerName);

			var viewModel = new Error(requestedPath, exception, _httpContextAccessor.HttpContext.User, signInURL, signOutURL);
			return View(viewModel);
        }
    }
}
