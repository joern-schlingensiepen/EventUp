using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System.Web.Mvc;

namespace EventUpWebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Explore our success stories and find everything you need for your next event.";

            return View();
        }

        public ActionResult Contact()
        {
           return View();
        }

        private ApplicationUserManager _UserManager;

        private ApplicationUserManager UserManager =>
            _UserManager ?? (_UserManager =
                HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>());
    }
}