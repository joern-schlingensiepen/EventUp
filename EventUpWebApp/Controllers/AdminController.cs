using EventUpLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            var servicesWithReservedEvents = db.Services.Include("isBookedFor").OrderBy(service => service.Name).ToList();
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

        [HttpGet]
        public ActionResult DetailsService(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var service = db.Services.Find(id);

            if (service == null)
            {
                return HttpNotFound();
            }

            return View(service);
        }
    }
}