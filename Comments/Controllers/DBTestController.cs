using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comments.Models;
using Comments.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Comments.Controllers
{
    public class DBTestController : Controller
    {
        private readonly ConsultationsContext _consultationsContext;
        private readonly IConsultationService _consultationService;

        public DBTestController(ConsultationsContext consultationsContext, IConsultationService consultationService)
        {
            _consultationsContext = consultationsContext;
            _consultationService = consultationService;
        }

        // GET: DBTest
        public ActionResult Index()
        {
            return Json(_consultationService.GetAllConsultations());
        }

        //// GET: DBTest/Create
        public ActionResult Create()
        {
            _consultationsContext.Consultations.Add(
                new Consultation("some ref", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(30), "some title"));

            _consultationsContext.SaveChanges();

            return Content(_consultationsContext.Consultations.Count() + " rows");
        }

       
    }
}