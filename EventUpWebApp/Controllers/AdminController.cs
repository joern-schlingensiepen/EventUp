using EventUpLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventUpWebApp.Controllers
{
    public class AdminController : Controller
    {
        private Model1Container db = new Model1Container(); // Reemplaza YourDbContext con el nombre real de tu DbContext

        public AdminController()
        {
            db = new Model1Container(); // Reemplaza YourDbContext con el nombre real de tu DbContext
        }

        // GET: Admin
        public ActionResult Index()
        {
            var servicesWithReservedEvents = db.Services.Include("isBookedFor").ToList();
            return View(servicesWithReservedEvents);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}