using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EventUpLib;
using EventUpWebApp.Models;
using System.Diagnostics;
using System.Web.Hosting;


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
                

                if (user == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (eventViewModel.Start_DateTime == null || eventViewModel.End_DateTime == null ||eventViewModel.NumberOfGuest == 0|| eventViewModel.City==null || eventViewModel.Name==null)
                {
                    ViewBag.TypEventOptions = GetTypEventOptions();
                    return View(eventViewModel);
                }

                
                if (!IsValidCity(eventViewModel.City))
                {
                    ModelState.AddModelError("City", "City is not valid.");
                    ViewBag.TypEventOptions = GetTypEventOptions();
                    ViewBag.isPlannedById = new SelectList(db.Users, "Id", "Name", eventViewModel.isPlannedById);
                    return View(eventViewModel);
                }

                // Event name already exists for this user?
                if (db.Events.Any(e => e.Name == eventViewModel.Name && e.isPlannedBy.Id == user.Id))
                {
                    ModelState.AddModelError("Name", "Event name already exists in your event list");
                    ViewBag.TypEventOptions = GetTypEventOptions();
                    ViewBag.isPlannedById = new SelectList(db.Users, "Id", "Name", eventViewModel.isPlannedById);
                    return View(eventViewModel);
                }

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

            eventViewModel.TypEventOptions = typEventOptions;
            ViewBag.TypEventOptions = typEventOptions;
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
            eventViewModel.TypEventOptions = GetTypEventOptions();
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

                if (!IsValidCity(eventViewModel.City))
                {
                    ModelState.AddModelError("City", "City is not valid.");
                    ViewBag.isPlannedById = new SelectList(db.Users, "Id", "Name", eventViewModel.isPlannedById);
                    return View(eventViewModel);
                }

               
                // Event name already exists for this user?
                if (db.Events.Any(e => e.Name == eventViewModel.Name && e.isPlannedBy.Id == user.Id && e.Id != eventViewModel.Id))
                {
                    ModelState.AddModelError("Name", "Event name already exists for this user.");
                    ViewBag.TypEventOptions = GetTypEventOptions();
                    ViewBag.isPlannedById = new SelectList(db.Users, "Id", "Name", eventViewModel.isPlannedById);
                    return View(eventViewModel);
                }

                existingEvent.isPlannedBy = user;
                // Actualizar el modelo existente con los valores del modelo vinculado
                if (TryUpdateModel(existingEvent, "", new string[] { "Name", "City", "Address", "NumberOfGuest", "Budget", "Typ_Event", "Start_DateTime", "End_DateTime", "SelectedTypEvent" }))
                {
                    db.SaveChanges();
                    return RedirectToAction("MyEvents");
                }
                
            }
            
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
                new SelectListItem { Text = "Wedding", Value = "Wedding" },
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
           
            var reservedServices = @event.have.ToList();

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


        // read cities from a file
        private List<string> LoadCitiesFromTextFile()
        {
            
            var filePath = HostingEnvironment.MapPath("~/Content/Resources/cities.txt"); 
            var citiesText = System.IO.File.ReadAllText(filePath);
            var cities = citiesText.Split(new[] { "\", \"" }, StringSplitOptions.RemoveEmptyEntries)
                                   .Select(city => city.Trim('\"'))
                                   .ToList();
            return cities;
        }

        //validation of city
        private bool IsValidCity(string city)
        {
            var cities = LoadCitiesFromTextFile();
            return cities.Contains(city);
        }

        
    }
}
