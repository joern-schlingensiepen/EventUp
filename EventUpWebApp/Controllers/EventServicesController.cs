using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;
using EventUpLib;
using EventUpWebApp.Models;
using System.Diagnostics;


namespace EventUpWebApp.Controllers
{
    public class EventServicesController : Controller
    {
        private Model1Container db = new Model1Container();

        [HttpGet]
        public ActionResult ListServices(string cityFilter, string typServiceFilter, string typEventFilter, int selectedEventId)
        {
            
            ViewBag.SelectedEventId = selectedEventId; 
            var allCities = db.Services.Select(s => s.City).Distinct().ToList();
            var selectedCity = string.IsNullOrEmpty(cityFilter) ? GetSelectedEvent(selectedEventId)?.City : cityFilter;

           
            ViewBag.CityList = new SelectList(allCities, selectedCity);

            return View(BuildServiceViewModels(cityFilter, typServiceFilter, typEventFilter, selectedEventId));
        }

        [HttpPost]
        public ActionResult ListServicesPost(string cityFilter, string typServiceFilter, string typEventFilter, int selectedEventId)
        {
            Debug.WriteLine("entra al post");
            ViewBag.SelectedEventId = selectedEventId;
            ViewBag.SelectedEventName = GetSelectedEvent(selectedEventId)?.Name;
            
           
            var allCities = db.Services.Select(s => s.City).Distinct().ToList();

            
            ViewBag.CityList = new SelectList(allCities);

          
            var serviceViewModels = BuildServiceViewModels(cityFilter, typServiceFilter, typEventFilter, selectedEventId);
            return View("ListServices", serviceViewModels);
        }

        private List<ServiceViewModel> BuildServiceViewModels(string cityFilter, string typServiceFilter, string typEventFilter, int selectedEventId)
        {
           
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

        // Dropdown menu options for Typ_Service
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

        // Dropdown menu options for Typ_Event
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
            
           
            var currentEvent = GetSelectedEvent(selectedEventId);
           
            if (currentEvent == null)
            {
                return HttpNotFound();
            }

            if (selectedServices != null && selectedServices.Any())
            {
                
                var selectedServiceIds = selectedServices.Where(s => s.IsSelected).Select(s => s.Id).ToList();

                
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
            
            return db.Events.Find(eventId);
        }

        private Event GetCurrentUserEvent()
        {
            var userName = User.Identity.Name;
            User user = db.Users.FirstOrDefault(u => u.Email == userName);

            
            var userEvent = db.Events.FirstOrDefault(e => e.isPlannedBy != null && e.isPlannedBy.Id == user.Id);

            return userEvent;
        }


        public ActionResult ReservedServices(int id) 
        {
            
            var selectedEvent = db.Events.Include(e => e.have).FirstOrDefault(e => e.Id == id);
           
            ViewBag.SelectedEventCity = selectedEvent?.City;

            if (selectedEvent == null)
            {
                
                return HttpNotFound();
            }

           
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

            
            double totalEventValue = reservedServicesViewModel.Sum(service => service.TotalEventValue);
            ViewBag.TotalEventValue = totalEventValue;
            ViewBag.Budget = selectedEvent.Budget;
            ViewBag.SelectedEventId = id; 
            ViewBag.SelectedEventName = selectedEvent.Name;

          

            return View(reservedServicesViewModel);
            
            
        }

        private double CalculateTotalEventValue(Service service, Event selectedEvent)
        {
            
            double fixCost = service.FixCost ?? 0.0;
            double hourCost = service.HourCost ?? 0.0;
            double personCost = service.PersonCost ?? 0.0;

            
            DateTime startDateTime = selectedEvent.Start_DateTime;
            DateTime endDateTime = selectedEvent.End_DateTime;
            int numberOfGuest = selectedEvent.NumberOfGuest;

            
            double totalValue = fixCost + (hourCost * (endDateTime - startDateTime).TotalHours) + (personCost * numberOfGuest); 

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
            ViewBag.SelectedEventId = selectedEventId; 
            return View(service);
        }

        public ActionResult DetailsEvent(int? id, int? selectedServiceId)
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
            ViewBag.SelectedServiceId = selectedServiceId;
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
            ViewBag.SelectedEventId = selectedEventId; 
            return View("Delete", service);
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            
            var currentEvent = GetCurrentUserEvent();

            if (currentEvent == null)
            {
                return HttpNotFound();
            }

            
            var serviceToRemove = currentEvent.have.FirstOrDefault(s => s.Id == id);

            if (serviceToRemove != null)
            {
                
                currentEvent.have.Remove(serviceToRemove);
                db.SaveChanges(); 

                return RedirectToAction("ReservedServices", new { id = currentEvent.Id });
            }

            return HttpNotFound();
        }

        public ActionResult ServiceIsBookedFor(int id) 
        {
            {
                var service = db.Services.Find(id);

                if (service == null)
                {
                    return HttpNotFound();
                }

                ViewBag.SelectedServiceName = service.Name;
                ViewBag.SelectedEventId = service.isBookedFor;
                ViewBag.SelectedServiceId = id;
                

                var events = service.isBookedFor.ToList();

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