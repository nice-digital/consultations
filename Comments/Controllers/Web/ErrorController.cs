using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Comments.Controllers.Web
{
    /// <summary>
    /// This error controller is only shown if there's a problem showing the error using react..
    /// </summary>
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return Content("This is the MVC error controller.");
        }
    }
}