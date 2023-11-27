using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
            return View("MyServices", service);
        }

        // GET: Services/Create
        public ActionResult Create()
        {
            ViewBag.isOfferedById = new SelectList(db.Users, "Id", "Name");
            return View();
        }

        // POST: Services/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Address,Typ_Service,Typ_Event,Capacity,FixCost,HourCost,PersonCost,City,More,isOfferedById")] Service service)
        {
            if (ModelState.IsValid)
            {

                var userId = User.Identity.GetUserId();
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var aspNetUser = userManager.FindById(userId);
                              
                User user = null;

                if (aspNetUser != null)
                {
                    var userEmail = aspNetUser.Email;
                    user = db.Users.FirstOrDefault(u => u.Email == userEmail);

                    if (user == null)
                    {
                        Debug.WriteLine("user is null");
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                }

                service.isOfferedBy = user;
                // Agregar el servicio a la base de datos
                db.Services.Add(service);
                db.SaveChanges();

                // Redirigir a la acción que muestra los servicios del usuario
                return RedirectToAction("MyServices");

            }

            ViewBag.isOfferedById = new SelectList(db.Users, "Id", "Name", service.isOfferedById);
            return View(service);
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
                db.Entry(service).State = EntityState.Modified;
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
            return View("MyServices", service);
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
        public ActionResult MyServices()
        {
            // Obtener el Id del usuario actual
            var userId = User.Identity.GetUserId();

            // Crear una instancia de UserManager (se refiere a la gestión de usuarios en ASP.NET Identity)
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            // Utilizar UserManager para buscar el usuario en la base de datos mediante el identificador
            var aspNetUser = userManager.FindById(userId);

            // Declarar la variable user fuera del bloque if
            User user = null;

            if (aspNetUser != null)
            {
                // Ahora, "aspNetUser" es el usuario recuperado de ASP.NET Identity
                // Puedes acceder a propiedades como Email, UserName, etc.
                var userEmail = aspNetUser.Email;

                // Imprimir el correo electrónico asociado al usuario actual
                Debug.WriteLine($"User Email: {userEmail}");

                // Buscar el usuario en tu base de datos
                user = db.Users.FirstOrDefault(u => u.Email == userEmail);

                if (user == null)
                {
                    Debug.WriteLine("user is null");
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }

            // Mover esta línea fuera del bloque if
            ViewBag.OffersIds = user.offers.Select(o => o.Id).ToList();
            return View("MyServices", user.offers.ToList());
        }

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
