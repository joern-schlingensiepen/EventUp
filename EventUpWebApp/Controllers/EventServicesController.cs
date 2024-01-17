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
            
            ViewBag.SelectedEventId = selectedEventId; //permite que la accion almacene el id del evento seleccionado
            // Obtén todas las ciudades disponibles desde la base de datos
            var allCities = db.Services.Select(s => s.City).Distinct().ToList();

            // Si hay una ciudad filtrada, úsala; de lo contrario, usa la ciudad del evento seleccionado
            var selectedCity = string.IsNullOrEmpty(cityFilter) ? GetSelectedEvent(selectedEventId)?.City : cityFilter;

            // Construye la lista de ciudades sin aplicar el filtro
            ViewBag.CityList = new SelectList(allCities, selectedCity);

            return View(BuildServiceViewModels(cityFilter, typServiceFilter, typEventFilter, selectedEventId));
        }

        [HttpPost]
        public ActionResult ListServicesPost(string cityFilter, string typServiceFilter, string typEventFilter, int selectedEventId)
        {
            Debug.WriteLine("entra al post");
            ViewBag.SelectedEventId = selectedEventId;
            ViewBag.SelectedEventName = GetSelectedEvent(selectedEventId)?.Name;
            
            // Obtén todas las ciudades disponibles desde la base de datos
            var allCities = db.Services.Select(s => s.City).Distinct().ToList();

            // Restablece la lista de ciudades con el filtro aplicado
            ViewBag.CityList = new SelectList(allCities);

            // Filtra la lista de servicios según los filtros seleccionados
            var serviceViewModels = BuildServiceViewModels(cityFilter, typServiceFilter, typEventFilter, selectedEventId);
            return View("ListServices", serviceViewModels);
        }

        private List<ServiceViewModel> BuildServiceViewModels(string cityFilter, string typServiceFilter, string typEventFilter, int selectedEventId)
        {
            // Lógica común para obtener la lista de servicios
            var filteredServices = db.Services.ToList();

            if (!string.IsNullOrEmpty(cityFilter) && cityFilter != "All Cities")
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

            // Considera todos los filtros al mismo tiempo
            if (!string.IsNullOrEmpty(cityFilter) && !string.IsNullOrEmpty(typServiceFilter) && !string.IsNullOrEmpty(typEventFilter))
            {
                filteredServices = filteredServices
                    .Where(s => s.City == cityFilter && s.Typ_Service == typServiceFilter && s.Typ_Event == typEventFilter)
                    .ToList();
            }
            
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
           
            if (currentEvent == null)
            {
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


        public ActionResult ReservedServices(int id) // muestra los servicios reservados para un evento
        {
            
            var selectedEvent = db.Events.Include(e => e.have).FirstOrDefault(e => e.Id == id);
            // Aquí estableces la ViewBag.SelectedEventCity con la ciudad del evento actual
            ViewBag.SelectedEventCity = selectedEvent?.City;

            if (selectedEvent == null)
            {
                
                return HttpNotFound();
            }

            // Mapear la lista de servicios a ServiceViewModel
            var reservedServicesViewModel = selectedEvent.have.Select(service =>
            {
               
                var viewModel = new ServiceViewModel
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
                    isOfferedById = service.isOfferedById,
                    TotalEventValue = CalculateTotalEventValue(service, selectedEvent)
                };
                
                return viewModel;
            }).ToList();

            // Calcular el valor total del evento sumando los valores de todos los servicios
            double totalEventValue = reservedServicesViewModel.Sum(service => service.TotalEventValue);
            ViewBag.TotalEventValue = totalEventValue;
            ViewBag.Budget = selectedEvent.Budget;
            ViewBag.SelectedEventId = id; //permite que la accion almacene el id del evento seleccionado
            ViewBag.SelectedEventName = selectedEvent.Name;

          

            return View(reservedServicesViewModel);
            
            
        }

        private double CalculateTotalEventValue(Service service, Event selectedEvent)
        {
            // Obtener los valores necesarios del servicio, si es null , retorna 0
            double fixCost = service.FixCost ?? 0.0;
            double hourCost = service.HourCost ?? 0.0;
            double personCost = service.PersonCost ?? 0.0;

            // Obtener los valores necesarios del evento
            DateTime startDateTime = selectedEvent.Start_DateTime;
            DateTime endDateTime = selectedEvent.End_DateTime;
            int numberOfGuest = selectedEvent.NumberOfGuest;

            // Realizar los cálculos necesarios
            double totalValue = fixCost + (hourCost * (endDateTime - startDateTime).TotalHours) + (personCost * numberOfGuest); //calcula el valor total del evento sin problema

            return totalValue;
        }

            

        // GET: Services/Details/5
        [HttpGet]
        public ActionResult Details(int? id, int? selectedEventId)  
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
            ViewBag.SelectedEventId = selectedEventId; //permite que la accion almacene el id del evento seleccionado
            return View("Details", service);
        }

        public ActionResult DetailsEvent(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var @event = db.Events.Find(id);

            if (@event == null)
            {
                return HttpNotFound();
            }

            return View(@event);
        }
        // GET: Services/Delete/5
        public ActionResult Delete(int? id, int? selectedEventId)
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
            ViewBag.SelectedEventId = selectedEventId; //permite que la accion almacene el id del evento seleccionado
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

        public ActionResult ServiceIsBookedFor(int id) //muestra los eventos para los que ha sido reservado el servicio
        {
            {
                var service = db.Services.Find(id);

                if (service == null)
                {
                    return HttpNotFound();
                }

                ViewBag.SelectedServiceName = service.Name;
                ViewBag.SelectedEventId = service.Id;
                ViewBag.SelectedServiceId = id;

                var events = service.isBookedFor.ToList();
                // Calcular el valor total del servicio y guardarlo en ViewBag
                double totalServiceValue = CalculateTotalServiceValue(events, service);
                ViewBag.TotalServiceValue = totalServiceValue;

                return View(events);

            }
        }
        private double CalculateTotalServiceValue(List<Event> events, Service selectedService)
        {
            double totalValue = 0.0;

            foreach (var selectedEvent in events)
            {
                double fixCost = selectedService.FixCost ?? 0.0;
                double hourCost = selectedService.HourCost ?? 0.0;
                double personCost = selectedService.PersonCost ?? 0.0;

                DateTime startDateTime = selectedEvent.Start_DateTime;
                DateTime endDateTime = selectedEvent.End_DateTime;
                int numberOfGuest = selectedEvent.NumberOfGuest;

                totalValue += fixCost + (hourCost * (endDateTime - startDateTime).TotalHours) + (personCost * numberOfGuest);
            }

            return totalValue;
        }
    }
}