using EventUpLib;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace EventUpWebApp.Controllers
{
    public class AdminController : Controller
    {
        private Model1Container db = new Model1Container(); 

        public AdminController()
        {
            db = new Model1Container(); 
        }

        // GET: Admin
        public ActionResult Index()
        {
            
            ViewBag.NoOfServices = db.Services.Count();
            ViewBag.NoOfEvents = db.Events.Count();

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