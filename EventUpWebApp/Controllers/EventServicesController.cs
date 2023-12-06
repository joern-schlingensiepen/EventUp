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
using System.Security.Cryptography;

namespace EventUpWebApp.Controllers
{
    public class EventServicesController : Controller
    {
        private Model1Container db = new Model1Container();

        [HttpGet]
        public ActionResult ListServices(string cityFilter, string typServiceFilter, string typEventFilter, int selectedEventId)
        {
            Debug.WriteLine("entra al get");
            return View(BuildServiceViewModels(cityFilter, typServiceFilter, typEventFilter, selectedEventId));
        }

        [HttpPost]
        public ActionResult ListServicesPost(string cityFilter, string typServiceFilter, string typEventFilter, int selectedEventId)
        {
            Debug.WriteLine("entra al post");
            ViewBag.SelectedEventId = selectedEventId;
            ViewBag.SelectedEventName = GetSelectedEvent(selectedEventId)?.Name;

            var serviceViewModels = BuildServiceViewModels(cityFilter, typServiceFilter, typEventFilter, selectedEventId);
            return View(serviceViewModels);
        }

        private List<ServiceViewModel> BuildServiceViewModels(string cityFilter, string typServiceFilter, string typEventFilter, int selectedEventId)
        {
            // Lógica común para obtener la lista de servicios
            var filteredServices = db.Services.ToList();

            if (!string.IsNullOrEmpty(cityFilter))
            {
                filteredServices = filteredServices.Where(s => s.City == cityFilter).ToList();
            }

            if (!string.IsNullOrEmpty(typServiceFilter))
            {
                filteredServices = filteredServices.Where(s => s.Typ_Service == typServiceFilter).ToList();
            }

            if (!string.IsNullOrEmpty(typEventFilter))
            {
                filteredServices = filteredServices.Where(s => s.Typ_Event == typEventFilter).ToList();
            }

            ViewBag.CityFilter = GetSelectedEvent(selectedEventId)?.City;
            ViewBag.CityList = new SelectList(filteredServices.Select(s => s.City).Distinct().ToList());
            ViewBag.TypServiceOptions = new SelectList(GetTypServiceOptions(), "Value", "Text", typServiceFilter);
            ViewBag.TypEventOptions = new SelectList(GetTypEventOptions(), "Value", "Text", typEventFilter);
            ViewBag.SelectedEventId = selectedEventId;

            return filteredServices.Select(service => new ServiceViewModel
            {
                Id = service.Id,
                Name = service.Name,
                Address = service.Address,
                Typ_Service = service.Typ_Service,
                Typ_Event = service.Typ_Event,
                Capacity = service.Capacity,
                FixCost = service.FixCost,
                HourCost = service.HourCost,
                PersonCost = service.PersonCost,
                City = service.City,
                More = service.More,
                isOfferedById = service.isOfferedById
            }).ToList();
        }
        // Método para obtener las opciones de la lista desplegable
        private List<SelectListItem> GetTypServiceOptions()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "Decoration", Value = "Decoration" },
                new SelectListItem { Text = "Entertainment", Value = "Entertainment" },
                new SelectListItem { Text = "Food", Value = "Food" },
                new SelectListItem { Text = "Music", Value = "Music" },
                new SelectListItem { Text = "Place", Value = "Place" },
                new SelectListItem { Text = "Photography", Value = "Photography" },
                new SelectListItem { Text = "Transport", Value = "Transport" },
                new SelectListItem { Text = "Other", Value = "Other" },
            };
        }

        // Método para obtener las opciones de la lista desplegable
        private List<SelectListItem> GetTypEventOptions()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "Baby shower", Value = "Baby shower" },
                new SelectListItem { Text = "Birthday", Value = "Birthday" },
                new SelectListItem { Text = "Children's birthday", Value = "Children's birthday" },
                new SelectListItem { Text = "Concerts", Value = "Concerts" },
                new SelectListItem { Text = "Corporate event", Value = "Corporate event" },

                new SelectListItem { Text = "All", Value = "All" },
                new SelectListItem { Text = "Other", Value = "Other" },
            };
        }

        [HttpPost]
        [ActionName("SaveServicesForEvent")]
        public ActionResult SaveServicesForEventPost(IEnumerable<ServiceViewModel> selectedServices, int selectedEventId)
        {
            
            // Obtener el evento actual (puedes ajustar cómo obtienes el evento según tu lógica)
            var currentEvent = GetSelectedEvent(selectedEventId);
            Debug.WriteLine($"SaveServicesForEvent - selectedEventId: {selectedEventId}");

            if (currentEvent == null)
            {
                Debug.WriteLine("SaveServicesForEvent - currentEvent is null");
                return HttpNotFound();
            }

            if (selectedServices != null && selectedServices.Any())
            {
                // Obtén las IDs de los servicios seleccionados
                var selectedServiceIds = selectedServices.Where(s => s.IsSelected).Select(s => s.Id).ToList();

                // Obtén la lista de servicios seleccionados desde la base de datos
                var selectedServicesList = db.Services.Where(s => selectedServiceIds.Contains(s.Id)).ToList();


                foreach (var service in selectedServicesList)
                {
                    if (!currentEvent.have.Any(e => e.Id == service.Id))
                    {
                        currentEvent.have.Add(service);
                    }
                }

                db.SaveChanges();
            }

            return RedirectToAction("ReservedServices", new { id = currentEvent.Id });
        }

        private Event GetSelectedEvent(int eventId)
        {
            // Obtener el evento por el Id proporcionado
            return db.Events.Find(eventId);
        }

        private Event GetCurrentUserEvent()
        {
            var userName = User.Identity.Name;
            User user = db.Users.FirstOrDefault(u => u.Email == userName);

            // Buscar el evento asociado al usuario actual
            var userEvent = db.Events.FirstOrDefault(e => e.isPlannedBy != null && e.isPlannedBy.Id == user.Id);

            return userEvent;
        }


        public ActionResult ReservedServices(int id) //muestra los servicios reservados para un evento
        {
            
            var selectedEvent = db.Events.Include(e => e.have).FirstOrDefault(e => e.Id == id);

            if (selectedEvent == null)
            {
                return HttpNotFound();
            }

            // Mapear la lista de servicios a ServiceViewModel
            var reservedServicesViewModel = selectedEvent.have.Select(service => new ServiceViewModel
            {
                Id = service.Id,
                Name = service.Name,
                Address = service.Address,
                Typ_Service = service.Typ_Service,
                //TypServiceOptions = GetTypServiceOptions(),
                Typ_Event = service.Typ_Event,
                //TypEventOptions = GetTypEventOptions(),
                Capacity = service.Capacity,
                FixCost = service.FixCost,
                HourCost = service.HourCost,
                PersonCost = service.PersonCost,
                City = service.City,
                More = service.More,
                isOfferedById = service.isOfferedById
            }).ToList();
            ViewBag.SelectedEventId = id;
            ViewBag.SelectedEventName = selectedEvent.Name;
            // Pasar la lista de servicios asociados a la vista
            return View(reservedServicesViewModel);
        }

        // GET: Services/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = db.Services.Find(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            return View("Details", service);
        }
        // GET: Services/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = db.Services.Find(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            return View("Delete", service);
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            // Obtén el evento actual del usuario
            var currentEvent = GetCurrentUserEvent();

            if (currentEvent == null)
            {
                return HttpNotFound();
            }

            // Busca el servicio en la lista de servicios del evento
            var serviceToRemove = currentEvent.have.FirstOrDefault(s => s.Id == id);

            if (serviceToRemove != null)
            {
                // Elimina el servicio solo del evento, no de la base de datos
                currentEvent.have.Remove(serviceToRemove);
                db.SaveChanges(); // Guarda los cambios en la base de datos

                return RedirectToAction("ReservedServices", new { id = currentEvent.Id });
            }

            return HttpNotFound(); // El servicio no se encontró en la lista del evento
        }
    }
}