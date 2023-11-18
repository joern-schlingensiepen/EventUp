using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EventUpLib;
using Microsoft.AspNet.Identity;

namespace EventUpWebApp.Controllers
{

    public class CostumerController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: Costumer/Edit/5
        [AllowAnonymous]
        public ActionResult Edit(int? id)
        {
            var userName = User.Identity.GetUserName();
            Person user = db.Persons.FirstOrDefault(
                u => u.Email == userName);

            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(user);
        }
        [AllowAnonymous]
        // POST: Costumer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FamilyName,Name,Email,TelephoneNumber,Role")] Person user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}