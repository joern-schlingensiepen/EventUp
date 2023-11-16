using EventUpWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventUpWebApp.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult SignIn()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignIn(AccountViewModels model)
        {
            // Implementa la lógica de autenticación aquí.

            // Si la autenticación es exitosa, redirige a la página principal.
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Register(AccountViewModels model)
        {
            // Implementa la lógica de registro aquí.

            // Después del registro, redirige a la página de inicio de sesión.
            return RedirectToAction("SignIn");
        }
    }
}