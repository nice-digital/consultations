using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Comments.Controllers
{
    [Route("consultations")]
    public class ConsultationsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}