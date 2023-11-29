using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Net;
using EventUpLib;
using EventUpWebApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Diagnostics;

namespace EventUpWebApp.Controllers
{
    public class EventServicesController : Controller
    {
        private Model1Container db = new Model1Container();

        
        public ActionResult ListServices()
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

            // Obtener todos los servicios disponibles
            var allServices = db.Services.ToList();

            return View(allServices);
        }

        [HttpPost]
        public ActionResult SaveServicesForEvent(IEnumerable<int> selectedServices)
        {
            // Obtener el evento actual (puedes ajustar cómo obtienes el evento según tu lógica)
            var currentEvent = GetCurrentUserEvent();  // Necesitarás implementar esta función

            if (currentEvent == null)
            {
                return HttpNotFound();
            }

            // Obtener los servicios seleccionados
            var selectedServicesList = db.Services.Where(s => selectedServices.Contains(s.Id)).ToList();

            // Agregar cada servicio al evento actual
            foreach (var service in selectedServicesList)
            {
                currentEvent.have.Add(service);
            }

            // Guardar cambios en la base de datos
            db.SaveChanges();

            return RedirectToAction("ReservedServices", new { id = currentEvent.Id });
        }

        private Event GetCurrentUserEvent()
        {
            var userName = User.Identity.Name;
            User user = db.Users.FirstOrDefault(u => u.Email == userName);

            // Obtener el ID del usuario actual
            //string userId = User.Identity.GetUserId();

            /*if (string.IsNullOrEmpty(userId))
            {
                // El usuario no está autenticado
                return null;
            }
            User user = GetUserById(userId);*/
           
            // Buscar el evento asociado al usuario actual
            var userEvent = db.Events.FirstOrDefault(e => e.isPlannedBy != null && e.isPlannedBy.Id == user.Id);

            return userEvent;
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
        public ActionResult ReservedServices(int id) //muestra los servicios reservados para un evento
        {

            var selectedEvent = db.Events.Include(e => e.have).FirstOrDefault(e => e.Id == id);

            if (selectedEvent == null)
            {
                return HttpNotFound();
            }
            var reservedServices = selectedEvent.have; // Supongo que 'have' es la lista de servicios asociados al evento
            return View(reservedServices);
        }
    }
}