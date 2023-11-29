using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EventUpLib;
using EventUpWebApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

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
        public ActionResult MyServices()
        {
            var userName = User.Identity.Name;
            User user = db.Users.FirstOrDefault(u => u.Email == userName);
            //User user = GetUserById(User.Identity.GetUserId());

            if (user == null)
            {
                Debug.WriteLine("user is null");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.OffersIds = user.offers.Select(o => o.Id).ToList();
            return View("MyServices", user.offers.ToList());
        }

        // GET: Services/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = db.Services.Find(id);
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
            // Crear las opciones solo si es una solicitud GET
            if (HttpContext.Request.HttpMethod == "GET")
            {
                var typServiceOptions = new List<SelectListItem>
        {
            new SelectListItem { Text = "Decoration", Value = "Decoration" },
            new SelectListItem { Text = "Entertainment", Value = "Entertainment" },
            new SelectListItem { Text = "Food", Value = "Food" },
            new SelectListItem { Text = "Music", Value = "Music" },
            new SelectListItem { Text = "Place", Value = "Place" },
            new SelectListItem { Text = "Photography", Value = "Photography" },
            new SelectListItem { Text = "Transport", Value = "Transport" },
            new SelectListItem { Text = "Other", Value = "Other" },
            // Agrega más opciones según sea necesario
        };

                ViewBag.TypServiceOptions = typServiceOptions;
            }
            return View();
        }

        // POST: Services/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ServiceViewModel serviceViewModel)
        {

            serviceViewModel.TypServiceOptions = ViewBag.TypServiceOptions as List<SelectListItem>;

            if (ModelState.IsValid)
            {
                var userName = User.Identity.Name;
                User user = db.Users.FirstOrDefault(u => u.Email == userName);
                //User user = GetUserById(User.Identity.GetUserId());

                if (user == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                

                var service = new Service
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
            return View(serviceViewModel);
        }

        // GET: Services/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = db.Services.Find(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            ViewBag.isOfferedById = new SelectList(db.Users, "Id", "Name", service.isOfferedById);
            return View(service);
        }

        // POST: Services/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Address,Typ_Service,Typ_Event,Capacity,FixCost,HourCost,PersonCost,City,More,isOfferedById")] Service service)
        {
            if (ModelState.IsValid)
            {
                
                var existingService = db.Services.Find(service.Id);
                

                if (existingService == null)
                {
                    return HttpNotFound();
                }
                var userName = User.Identity.Name;
                User user = db.Users.FirstOrDefault(u => u.Email == userName);
                //User user = GetUserById(User.Identity.GetUserId());

                if (user == null)
                {
                  return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                existingService.isOfferedBy = user;
                db.Entry(existingService).CurrentValues.SetValues(service);
                db.SaveChanges();


                return RedirectToAction("MyServices");
            }

            ViewBag.isOfferedById = new SelectList(db.Users, "Id", "Name", service.isOfferedById);
            return View(service);
        }

        // GET: Services/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = db.Services.Find(id);
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
            Service service = db.Services.Find(id);
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

        public ActionResult ListServices(string cityFilter, string typServiceFilter, string typEventFilter)
        {
            // Obtener ciudades únicas de la base de datos
            var uniqueCities = db.Services.Select(s => s.City).Distinct().ToList();
            ViewBag.CityList = new SelectList(uniqueCities);

            // Obtener tipos de servicio únicos de la base de datos
            var uniqueTypServices = db.Services.Select(s => s.Typ_Service).Distinct().ToList();
            ViewBag.Typ_ServiceList = new SelectList(uniqueTypServices);

            // Obtener tipos de evento únicos de la base de datos
            var uniqueTypEvents = db.Services.Select(s => s.Typ_Event).Distinct().ToList();
            ViewBag.Typ_EventList = new SelectList(uniqueTypEvents);

            // Filtrar servicios según los parámetros de filtro
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

            // Devolver la vista con los servicios filtrados
            return View(filteredServices.ToList());
        }
    }
}
