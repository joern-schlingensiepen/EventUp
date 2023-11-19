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


namespace EventUpWebApp.Controllers
{
    public class UserController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public UserController()
        {
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


        //
        // GET: /User/Email_Password
        [AllowAnonymous]
        public ActionResult Email_Password()
        {
            return View();
        }

        //
        // POST: /User/Email_Password
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Email_Password(Email_PasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var ctx = new EventUpLib.Model1Container())
                {
                    if (ctx.Users.Any(s => s.Email == model.Email))
                    {
                        AddErrors(
                            new IdentityResult(new string[] { "EMail schon vergeben!" })
                        );
                        return View(model);
                    }

                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        EventUpLib.User aUser = new User()
                        {
                            Email = model.Email
                        };

                        //try { 
                      
                        ctx.Users.Add(aUser);
                        ctx.SaveChanges();
                    
                        //}
                        //catch (Exception e)
                        //{
                        //    Console.WriteLine(e.Message);
                        //}
                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                        return RedirectToAction("Create", "User");
                    }
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }



        private Model1Container db = new Model1Container();
        // GET: /User/Create
        public ActionResult Create()
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

        // POST: Users/Create
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

                // Asigna el rol correspondiente
                if (user.Role_Admin)
                {
                    ViewBag.SelectedRole = "Admin";
                }
                else if (user.Role_Supplier)
                {
                    ViewBag.SelectedRole = "Supplier";
                }
                else if (user.Role_Planner)
                {
                    ViewBag.SelectedRole = "Planner";
                }

                db.Users.Add(user);
                db.SaveChanges();

                return RedirectToAction("Index", "Home");
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
        public ActionResult Edit([Bind(Include = "Id,Name,Familyname,Street,City,PostCode,EMail,RoleCostumer,RoleAdministrator,RoleStaff")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
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