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

namespace EventUpWebApp.Controllers
{
    public class EventsController : Controller
    {
        private Model1Container db = new Model1Container();

        public ActionResult Index()
        {
           return View(db.Events.ToList());
        }

        public ActionResult MyEvents()
        {
            var userName = User.Identity.Name;
            User user = db.Users.FirstOrDefault(u => u.Email == userName);
            //User user = GetUserById(User.Identity.GetUserId());

            if (user == null)
            {
                Debug.WriteLine("user is null");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.PlansIds = user.plans.Select(o => o.Id).ToList();
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
                var userName = User.Identity.Name;
                User user = db.Users.FirstOrDefault(u => u.Email == userName);
                //User user = GetUserById(User.Identity.GetUserId());

                if (user == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                @event.isPlannedBy = user;
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
            return View(@event);
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
                var existingEvent= db.Events.Find(@event.Id); 

                if (existingEvent == null)
                {
                    return HttpNotFound();
                }

                var userName= User.Identity.Name;
                User user = db.Users.FirstOrDefault(u => u.Email == userName); //GetUserById(User.Identity.GetUserId());
                if (user == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                existingEvent.isPlannedBy = user;
                db.Entry(existingEvent).CurrentValues.SetValues(@event);
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
            return View("Delete", @event);
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
