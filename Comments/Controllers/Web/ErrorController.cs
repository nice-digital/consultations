using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comments.ViewModels;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Comments.Controllers.Web
{
	/// <summary>
	/// This error controller is only shown if there's a problem showing the error using react..
	/// </summary>
	[Route("error")]
	public class ErrorController : Controller
    {
        public ActionResult Index()
        {
	        var reFeat = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
	        string requestedPath = null;
	        if (reFeat != null)
	        {
		        requestedPath = reFeat.OriginalPath;
	        }

			var ehFeat = HttpContext.Features.Get<IExceptionHandlerFeature>();
	        Exception exception = null;
	        if (ehFeat != null)
	        {
		        exception = ehFeat.Error;
	        }

			var viewModel = new Error(requestedPath, exception);
			return View(viewModel);
        }
    }
}
