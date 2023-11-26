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
            return View(service);
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
                db.Services.Add(service);
                db.SaveChanges();
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
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
            return View(service);
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Service service = db.Services.Find(id);
            db.Services.Remove(service);
            db.SaveChanges();
            return RedirectToAction("Index");
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
