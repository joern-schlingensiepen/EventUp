using Microsoft.AspNet.Identity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EventUpLib;
using EventUpWebApp.Controllers.Helpers;

namespace EventUpWebApp.Models
{
    public class RegisterController : Controller
    {
        private Model1Container db = new Model1Container();

        // GET: Costumer/Edit/5
        public ActionResult Edit(int? id)
        {
            var userName = User.Identity.GetUserName();
            User user = db.Users.FirstOrDefault(
                u => u.Email == userName);

            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(user);
        }

        // POST: Costumer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,FamilyName, TelephoneNumber, Email, Role_Admin, Role_Supplier, Role_Planner")] User user)
        {
            if (ModelState.IsValid)
            {
                
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                string selectedRole = UserRoleHelper.GetSelectedRole(user);
                Response.Cookies.Add(new HttpCookie("selectedRole", selectedRole));
                
                return RedirectToAction("Index", "Home", new { selectedRole = selectedRole });
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