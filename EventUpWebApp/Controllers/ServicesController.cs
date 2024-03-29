﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Services.Description;
using EventUpLib;
using EventUpWebApp.Models;


namespace EventUpWebApp.Controllers
{
    public class ServicesController : Controller
    {
        private Model1Container db = new Model1Container();
       
        // GET: Services
        public ActionResult Index()
        {
            return View(db.Services.ToList());
        }
        public ActionResult MyServices(int? selectedServiceId)
        {
            var userName = User.Identity.Name;
            User user = db.Users.FirstOrDefault(u => u.Email == userName);
            

            if (user == null)
            {
                Debug.WriteLine("user is null");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Ordenar los servicios por el número de eventos reservados en orden descendente
            //var sortedServices = user.offers.OrderByDescending(s => s.isBookedFor.Count).ToList();

            //ViewBag.OffersIds = sortedServices.Select(o => o.Id).ToList();
            //return View("MyServices", sortedServices);
            ViewBag.OffersIds = user.offers.Select(o => o.Id).ToList();
            // Pasa el selectedEventId directamente a la vista
            ViewBag.SelectedServiceId = selectedServiceId;
            return View("MyServices", user.offers.ToList());

        }

        // GET: Services/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            EventUpLib.Service service = db.Services.Find(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            return View("Details", service);
        }

        // GET: Services/Create
        public ActionResult Create()
        {
            ViewBag.isOfferedById = new SelectList(db.Users, "Id", "Name");
            ViewBag.TypServiceOptions = GetTypServiceOptions();
            ViewBag.TypEventOptions = GetTypEventOptions();
            return View();
        }

        // POST: Services/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ServiceViewModel serviceViewModel)
        {

            serviceViewModel.TypServiceOptions = GetTypServiceOptions();
            serviceViewModel.TypEventOptions = GetTypEventOptions();

            if (ModelState.IsValid)
            {
                var userName = User.Identity.Name;
                User user = db.Users.FirstOrDefault(u => u.Email == userName);
               

                if (user == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (!IsValidCity(serviceViewModel.City))
                {
                    ModelState.AddModelError("City", "City is not valid.");
                    ViewBag.TypServiceOptions = GetTypServiceOptions();
                    ViewBag.TypEventOptions = GetTypEventOptions();
                    ViewBag.isPlannedById = new SelectList(db.Users, "Id", "Name", serviceViewModel.isOfferedById);
                    return View(serviceViewModel);
                }
                // Service name already exists for this user?
                if (db.Services.Any(e => e.Name == serviceViewModel.Name && e.isOfferedBy.Id == user.Id))
                {
                    ModelState.AddModelError("Name", "Event name already exists in your event list");
                    ViewBag.TypServiceOptions = GetTypServiceOptions();
                    ViewBag.TypEventOptions = GetTypEventOptions();
                    ViewBag.isPlannedById = new SelectList(db.Users, "Id", "Name", serviceViewModel.isOfferedById);
                    return View(serviceViewModel);
                }

                // Validar que al menos uno de los costos sea diferente de null y mayor a cero
                if ((serviceViewModel.FixCost == null || serviceViewModel.FixCost <= 0) &&
                    (serviceViewModel.HourCost == null || serviceViewModel.HourCost <= 0) &&
                    (serviceViewModel.PersonCost == null || serviceViewModel.PersonCost <= 0))
                {
                    ModelState.AddModelError("", "At least one cost (FixCost, HourCost, or PersonCost) must be provided and greater than zero.");
                    ViewBag.TypServiceOptions = GetTypServiceOptions();
                    ViewBag.TypEventOptions = GetTypEventOptions();
                    return View(serviceViewModel);
                }

                var service = new EventUpLib.Service
                {
                    Id = serviceViewModel.Id,
                    Name = serviceViewModel.Name,
                    Address = serviceViewModel.Address,
                    Typ_Service = serviceViewModel.Typ_Service,
                    Typ_Event = serviceViewModel.Typ_Event, 
                    Capacity = serviceViewModel.Capacity,
                    FixCost = serviceViewModel.FixCost,
                    HourCost = serviceViewModel.HourCost,
                    PersonCost = serviceViewModel.PersonCost,
                    City = serviceViewModel.City,
                    More = serviceViewModel.More,
                    
                    isOfferedBy = user,

                };
                

                service.isOfferedBy = user;
                db.Services.Add(service);
                db.SaveChanges();

                return RedirectToAction("MyServices");

            }
            

            ViewBag.isOfferedById = new SelectList(db.Users, "Id", "Name", serviceViewModel.isOfferedById);
            ViewBag.TypServiceOptions = GetTypServiceOptions();  
            ViewBag.TypEventOptions = GetTypEventOptions();  
            return View(serviceViewModel);
        }

        // GET: Services/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventUpLib.Service service = db.Services.Find(id);
            if (service == null)
            {
                return HttpNotFound();
            }
           
            var typServiceOptions = GetTypServiceOptions();
            var typEventOptions = GetTypEventOptions();

           
            var serviceViewModel = new ServiceViewModel
            {
                Id = service.Id,
                Name = service.Name,
                Address = service.Address,
                Typ_Service = service.Typ_Service,
                Typ_Event = service.Typ_Event,
                Capacity = service.Capacity,
                FixCost = service.FixCost,
                HourCost = service.HourCost,
                PersonCost = service.PersonCost,
                City = service.City,
                More = service.More,
                isOfferedById = service.isOfferedBy.Id
            };



            serviceViewModel.TypEventOptions = typEventOptions;
            serviceViewModel.TypServiceOptions = typEventOptions;
            ViewBag.TypServiceOptions = GetTypServiceOptions();
            ViewBag.TypEventOptions = GetTypEventOptions();

            ViewBag.isOfferedById = new SelectList(db.Users, "Id", "Name", service.isOfferedById);
            return View(serviceViewModel);
        }

        // POST: Services/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ServiceViewModel serviceViewModel)
        {
            // Load options from the controller method
            serviceViewModel.TypServiceOptions = GetTypServiceOptions();
            serviceViewModel.TypEventOptions = GetTypEventOptions();

            if (ModelState.IsValid)
            {
                var existingService = db.Services.Find(serviceViewModel.Id);

                if (existingService == null)
                {
                    return HttpNotFound();
                }

                var userName = User.Identity.Name;
                User user = db.Users.FirstOrDefault(u => u.Email == userName);

                if (user == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (!IsValidCity(serviceViewModel.City))
                {
                    ModelState.AddModelError("City", "City is not valid.");
                    ViewBag.TypServiceOptions = GetTypServiceOptions();
                    ViewBag.TypEventOptions = GetTypEventOptions();
                    ViewBag.isPlannedById = new SelectList(db.Users, "Id", "Name", serviceViewModel.isOfferedById);
                    return View(serviceViewModel);
                }

                // Service name already exists for this user?
                if (db.Services.Any(e => e.Name == serviceViewModel.Name && e.isOfferedBy.Id == user.Id && e.Id != serviceViewModel.Id))
                {
                    ModelState.AddModelError("Name", "Service name already exists for this user.");
                    ViewBag.TypEventOptions = GetTypEventOptions();
                    ViewBag.TypServiceOptions = GetTypServiceOptions();
                    ViewBag.isPlannedById = new SelectList(db.Users, "Id", "Name", serviceViewModel.isOfferedById);
                    return View(serviceViewModel);
                }
                // Validar que al menos uno de los costos sea diferente de null y mayor a cero
                if ((serviceViewModel.FixCost == null || serviceViewModel.FixCost <= 0) &&
                    (serviceViewModel.HourCost == null || serviceViewModel.HourCost <= 0) &&
                    (serviceViewModel.PersonCost == null || serviceViewModel.PersonCost <= 0))
                {
                    ModelState.AddModelError("", "At least one cost (FixCost, HourCost, or PersonCost) must be provided and greater than zero.");
                    ViewBag.TypEventOptions = GetTypEventOptions();
                    ViewBag.TypServiceOptions = GetTypServiceOptions();
                    return View(serviceViewModel);
                }

                existingService.isOfferedBy = user;

                // Actualizar el modelo existente con los valores del modelo vinculado
                if (TryUpdateModel(existingService, "", new string[] { "Name", "Address", "Typ_Service", "Typ_Event", "Capacity", "FixCost", "HourCost", "PersonCost", "City", "More" }))
                {
                    db.SaveChanges();
                    return RedirectToAction("MyServices");
                }
            }
            
            ViewBag.isOfferedById = new SelectList(db.Users, "Id", "Name", serviceViewModel.isOfferedById);
            return View(serviceViewModel);
        }

        // Dropdown menu options
        private List<SelectListItem> GetTypServiceOptions()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "Decoration", Value = "Decoration" },
                new SelectListItem { Text = "Entertainment", Value = "Entertainment" },
                new SelectListItem { Text = "Food", Value = "Food" },
                new SelectListItem { Text = "Music", Value = "Music" },
                new SelectListItem { Text = "Place", Value = "Place" },
                new SelectListItem { Text = "Photography", Value = "Photography" },
                new SelectListItem { Text = "Transport", Value = "Transport" },
                new SelectListItem { Text = "Other", Value = "Other" },
            };
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

       

        // GET: Services/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventUpLib.Service service = db.Services.Find(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            return View("Delete", service);
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EventUpLib.Service service = db.Services.Find(id);
            
            var bookings = service.isBookedFor.ToList();

            
            foreach (var booking in bookings)
            {
                service.isBookedFor.Remove(booking);
            }
            db.Services.Remove(service);
            db.SaveChanges();
            return RedirectToAction("MyServices");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
       

      

        public ActionResult ListServices(string cityFilter, string typServiceFilter, string typEventFilter)
        {
            
            var uniqueCities = db.Services.Select(s => s.City).Distinct().ToList();
            ViewBag.CityList = new SelectList(uniqueCities);

            
            var uniqueTypServices = db.Services.Select(s => s.Typ_Service).Distinct().ToList();
            ViewBag.Typ_ServiceList = new SelectList(uniqueTypServices);

          
            var uniqueTypEvents = db.Services.Select(s => s.Typ_Event).Distinct().ToList();
            ViewBag.Typ_EventList = new SelectList(uniqueTypEvents);

            // filter services according parameters
            var filteredServices = db.Services.AsQueryable();

            if (!string.IsNullOrEmpty(cityFilter) && cityFilter != "All Cities")
            {
                filteredServices = filteredServices.Where(s => s.City == cityFilter);
             }

            if (!string.IsNullOrEmpty(typServiceFilter) && typServiceFilter != "All Services")
            {
                filteredServices = filteredServices.Where(s => s.Typ_Service == typServiceFilter);
            }

            if (!string.IsNullOrEmpty(typEventFilter) && typEventFilter != "All Events")
            {
             filteredServices = filteredServices.Where(s => s.Typ_Event == typEventFilter);
            }

            
            return View(filteredServices.ToList());
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
