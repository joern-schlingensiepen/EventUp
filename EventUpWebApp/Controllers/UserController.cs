using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using EventUpLib;
using System.Net;
using System.Collections.Generic;
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
                int rolesSelected = new List<bool> { user.Role_Admin, user.Role_Supplier, user.Role_Planner }.Count(r => r);

                if (rolesSelected != 1)
                {
                    ModelState.AddModelError("", "Select only one role.");
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
                int rolesSelected = new List<bool> { user.Role_Admin, user.Role_Supplier, user.Role_Planner }.Count(r => r);

                if (rolesSelected != 1)
                {
                    ModelState.AddModelError("", "Select only one role.");
                    return View(user);
                }
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
           
            foreach (var service in user.offers.ToList())
            {
                
                foreach (var booking in service.isBookedFor.ToList())
                {
                    service.isBookedFor.Remove(booking);
                }
                db.Services.Remove(service);
            }

            foreach (var plannedEvent in user.plans.ToList())
            {
                db.Events.Remove(plannedEvent);
            }

            

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

        // GET: User/Details
        public ActionResult Details(int? id)
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
            return View("Details", user);
        }


    }
}