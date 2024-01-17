using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EventUpLib;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using EventUpWebApp.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Diagnostics;
using System.Web.Services.Description;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;



namespace EventUpWebApp.Controllers
{
    public class EventsController : Controller
    {
        private Model1Container db = new Model1Container();

        public ActionResult Index()
        {
           return View(db.Events.ToList());
        }

        public ActionResult MyEvents(int? selectedEventId)
        {
            var userName = User.Identity.Name;
            EventUpLib.User user = db.Users.FirstOrDefault(u => u.Email == userName);
            //User user = GetUserById(User.Identity.GetUserId());

            if (user == null)
            {
                Debug.WriteLine("user is null");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.PlansIds = user.plans.Select(o => o.Id).ToList();
            // Pasa el selectedEventId directamente a la vista
            ViewBag.SelectedEventId = selectedEventId;

            return View("MyEvents", user.plans.ToList());
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View("Details",@event);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            ViewBag.isPlannedById = new SelectList(db.Users, "Id", "Name");
            ViewBag.TypEventOptions = GetTypEventOptions();
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EventViewModel eventViewModel)
       {
            eventViewModel.TypEventOptions = GetTypEventOptions();
            if (ModelState.IsValid)
            {
                var userName = User.Identity.Name;
                EventUpLib.User user = db.Users.FirstOrDefault(u => u.Email == userName);
                //User user = GetUserById(User.Identity.GetUserId());

                if (user == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (eventViewModel.Start_DateTime == null || eventViewModel.End_DateTime == null ||eventViewModel.NumberOfGuest == 0|| eventViewModel.City==null || eventViewModel.Name==null)
                {
                    ViewBag.TypEventOptions = GetTypEventOptions();
                    return View(eventViewModel);
                }
                 

                //mapeo de datos según el ViewModel antes de guardar los datos en sql
                var @event = new Event
                {
                    Id = eventViewModel.Id,
                    Name = eventViewModel.Name,
                    Address = eventViewModel.Address,
                    Typ_Event = eventViewModel.Typ_Event,
                    NumberOfGuest = eventViewModel.NumberOfGuest,
                    Budget = eventViewModel.Budget,
                    Start_DateTime = eventViewModel.Start_DateTime,
                    End_DateTime = eventViewModel.End_DateTime,
                    City = eventViewModel.City,

                    isPlannedBy = user,

                };

                @event.isPlannedBy = user;
                db.Events.Add(@event);
                db.SaveChanges();
                return RedirectToAction("MyEvents");
            }
            ViewBag.TypEventOptions = GetTypEventOptions();
            ViewBag.isPlannedById = new SelectList(db.Users, "Id", "Name", eventViewModel.isPlannedById);
            return View(eventViewModel);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }

            var typEventOptions = GetTypEventOptions();

            // Mapea los valores del servicio al modelo
            var eventViewModel = new EventViewModel
            {
                Id = @event.Id,
                Name = @event.Name,
                Address = @event.Address,
                Typ_Event = @event.Typ_Event,
                NumberOfGuest = @event.NumberOfGuest,
                Budget = @event.Budget,
                Start_DateTime = @event.Start_DateTime,
                End_DateTime = @event.End_DateTime,
                City = @event.City,

                isPlannedById = @event.isPlannedBy.Id,
            };

            // Asigna la lista desplegable al modelo
            eventViewModel.TypEventOptions = typEventOptions;
            ViewBag.isPlannedById = new SelectList(db.Users, "Id", "Name", @event.isPlannedById);
            return View(eventViewModel);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EventViewModel eventViewModel)
        {
            if (ModelState.IsValid)
            {
                var existingEvent= db.Events.Find(eventViewModel.Id); 

                if (existingEvent == null)
                {
                    return HttpNotFound();
                }

                var userName = User.Identity.Name;
                EventUpLib.User user = db.Users.FirstOrDefault(u => u.Email == userName);

                if (user == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                existingEvent.isPlannedBy = user;
                db.Entry(existingEvent).CurrentValues.SetValues(eventViewModel);
                db.SaveChanges();
                return RedirectToAction("MyEvents");
            }
            eventViewModel.TypEventOptions = GetTypEventOptions();
            ViewBag.isPlannedById = new SelectList(db.Users, "Id", "Name", eventViewModel.isPlannedById);
            return View(eventViewModel);
        }
        private List<SelectListItem> GetTypEventOptions()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "Baby shower", Value = "Baby shower" },
                new SelectListItem { Text = "Birthday", Value = "Birthday" },
                new SelectListItem { Text = "Children's birthday", Value = "Children's birthday" },
                new SelectListItem { Text = "Concerts", Value = "Concerts" },
                new SelectListItem { Text = "Corporate event", Value = "Corporate event" },
                new SelectListItem { Text = "All", Value = "All" },
                new SelectListItem { Text = "Other", Value = "Other" },
            };
        }
        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View("Delete", @event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Event @event = db.Events.Find(id);
            // Accede a la propiedad isBookedFor a través de la instancia de ServiceViewModel
            var reservedServices = @event.have.ToList();

            // Elimina las reservas asociadas
            foreach (var reservedService in reservedServices)
            {
                @event.have.Remove(reservedService);
            }
            db.Events.Remove(@event);
            db.SaveChanges();
            return RedirectToAction("MyEvents");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /*private User GetUserById(string userId)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var aspNetUser = userManager.FindById(userId);

            if (aspNetUser != null)
            {
                var userEmail = aspNetUser.Email;
                Debug.WriteLine($"userEmail: {userEmail}");

                // Verificar si el usuario existe en la base de datos
                var user = db.Users.FirstOrDefault(u => u.Email == userEmail);

                if (user != null)
                {
                    Debug.WriteLine($"User found in the database. UserID: {user.Id}");
                    return user;
                }
                else
                {
                    Debug.WriteLine("User not found in the database.");
                }
            }

            return null;
        }*/

     

    }
}
