using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Comments.Controllers.Web
{
	public class CommentingOnOtherThingsController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

	}
}
