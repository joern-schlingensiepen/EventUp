using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using EventUpLib;
using EventUpWebApp.Models;
using System.Net;
using System.Collections.Generic;
using System.Diagnostics;
using EventUpWebApp.Controllers.Helpers;


namespace EventUpWebApp.Controllers
{
    public class UserController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public UserController()
        {
        }

        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        public UserController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        private Model1Container db = new Model1Container();
        // GET: /User/Create (second register after email and password)
        public ActionResult Create()
        {
            var userName = User.Identity.GetUserName(); //se obtiene el nombre del usuario logueado
            User user = db.Users.FirstOrDefault(
                u => u.Email == userName);
                           

            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

          
            return View(user);
        } 

        // POST: Users/Create (second register after email and password)
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,FamilyName,Email,TelephoneNumber, Role_Admin, Role_Supplier, Role_Planner")] EventUpLib.User user)
        {
            if (ModelState.IsValid)
            {
                int rolesSeleccionados = new List<bool> { user.Role_Admin, user.Role_Supplier, user.Role_Planner }.Count(r => r);

                if (rolesSeleccionados != 1)
                {
                    ModelState.AddModelError("", "Debe seleccionar exactamente un rol.");
                    return View(user);
                }
                               
                db.Users.Add(user);
                db.SaveChanges();
                
               
                string selectedRole = UserRoleHelper.GetSelectedRole(user);
                              

                // Redireccionar según el rol
                return RedirectToAction("Index", "Home", new { selectedRole = selectedRole });
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,FamilyName,TelephoneNumber,Email,Role_Admin,Role_Supplier,Role_Planner")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

  
    }
}