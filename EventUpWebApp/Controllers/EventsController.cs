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

namespace EventUpWebApp.Controllers
{
    public class EventsController : Controller
    {
        private Model1Container db = new Model1Container();

        public ActionResult Index()
        {
           return View(db.Events.ToList());
        }

        // GET: Events
        //public ActionResult Index()
        //{
        //    var events = db.Events.Include(@ => @.isPlannedBy);
        //    return View(events.ToList());
        //
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
            return View("MyEvents",@event);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            ViewBag.isPlannedById = new SelectList(db.Users, "Id", "Name");
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,City,Address,NumberOfGuest,Budget,Typ_Event,Start_DateTime,End_DateTime,isPlannedById")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(@event);
                db.SaveChanges();
                return RedirectToAction("MyEvents");
            }

            ViewBag.isPlannedById = new SelectList(db.Users, "Id", "Name", @event.isPlannedById);
            return View(@event);
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
            ViewBag.isPlannedById = new SelectList(db.Users, "Id", "Name", @event.isPlannedById);
            return View("MyEvents", @event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,City,Address,NumberOfGuest,Budget,Typ_Event,Start_DateTime,End_DateTime,isPlannedById")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MyEvents");
            }
            ViewBag.isPlannedById = new SelectList(db.Users, "Id", "Name", @event.isPlannedById);
            return View(@event);
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
            return View("MyEvents", @event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Event @event = db.Events.Find(id);
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

        public ActionResult MyEvents()
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
            ViewBag.PlansIds = user.plans.Select(o => o.Id).ToList();
            return View("MyEvents", user.plans.ToList());
        }

    }
}
