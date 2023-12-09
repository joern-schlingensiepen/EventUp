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
            // Obtener el número de servicios en la plataforma
            ViewBag.NoOfServices = db.Services.Count();

            // Obtener el número de eventos en la plataforma
            ViewBag.NoOfEvents = db.Events.Count();

            // Obtener el número de coincidencias (matches)
            int noOfMatches = 0;

            var servicesWithReservedEvents = db.Services.Include("isBookedFor").OrderBy(service => service.Name).ToList();

            foreach (var service in servicesWithReservedEvents)
            {
                var reservedEvents = service.isBookedFor;
                if (reservedEvents != null && reservedEvents.Any())
                {
                    noOfMatches += reservedEvents.Select(e => e.Id).Distinct().Count();
                }
            }

            ViewBag.NoOfMatches = noOfMatches;



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

        [HttpGet]
        public ActionResult DetailsEvent(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var @event = db.Events.Find(id);

            if (@event == null)
            {
                return HttpNotFound();
            }



            return View(@event);
        }
    }
}