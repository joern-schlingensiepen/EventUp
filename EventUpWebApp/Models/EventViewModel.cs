using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using EventUpWebApp.Models;

namespace EventUpWebApp.Models
{
    public class EventViewModel
    {
        [Required(ErrorMessage = "Start hour is required")]
        [DisplayName("When Starts?")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm}")]
        public DateTime Start_DateTime { get; set; }

        [Required(ErrorMessage = "End hour is required")]
        [DisplayName("When Ends?")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm}")]
        public DateTime End_DateTime { get; set; }


        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }


        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }
        public string Address { get; set; }

        [Required(ErrorMessage = "Number of guest is required")]
        [DisplayName("Number of guest")]
        public int NumberOfGuest { get; set; }

        [DisplayName("Budget [Euro]")]
        public Nullable<double> Budget { get; set; }

        [Display(Name = "Event type")]
        public string Typ_Event { get; set; }
        public List<SelectListItem> TypEventOptions { get; set; }
        public string SelectedTypEvent { get; set; }

        public int isPlannedById { get; set; }

        public double ServiceValue { get; set; }


    }
}