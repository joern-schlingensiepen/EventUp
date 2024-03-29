﻿using EventUpLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace EventUpWebApp.Models
{
    public class ServiceViewModel

    {
        public int Id { get; set; }
        public bool IsSelected { get; set; } = false;

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
      
        public string Address { get; set; }


        [Required(ErrorMessage = "Typ Service is required")]
        [Display(Name = "Service type")]
        public string Typ_Service { get; set; }
        public List<SelectListItem> TypServiceOptions { get; set; }
        public string SelectedTypService { get; set; }


        [Display(Name = "Event type")]
        public string Typ_Event { get; set; }
        public List<SelectListItem> TypEventOptions { get; set; }
        

        // Constructor 
        public ServiceViewModel()
        {
            TypServiceOptions = new List<SelectListItem>();
            TypEventOptions = new List<SelectListItem>();
        }

        public Nullable<int> Capacity { get; set; }

        [DisplayName("Fix cost [Euro]")]
        public Nullable<double> FixCost { get; set; }

        [DisplayName("Hour cost [Euro/hour]")]
        public Nullable<double> HourCost { get; set; }

        [DisplayName("Person cost [Euro/person]")]
        public Nullable<double> PersonCost { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [DisplayName("More information")]
        public string More { get; set; }
        public int isOfferedById { get; set; }

        public virtual User isOfferedBy { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Event> isBookedFor { get; set; }

        public double TotalEventValue { get; set; }

        public string Description
        {
            get { return BuildDescription(); }
        }

        private string BuildDescription()
        {
            
            StringBuilder descriptionBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(Address))
            {
                descriptionBuilder.AppendLine($"Address: {Address}<br/>");
            }

            if (FixCost.HasValue)
            {
                descriptionBuilder.AppendLine($"Fix Cost[Euro]: {FixCost}<br/>");
            }

            if (HourCost.HasValue)
            {
                descriptionBuilder.AppendLine($"Hour Cost[Euro/h]: {HourCost}<br/>");
            }

            if (PersonCost.HasValue)
            {
                descriptionBuilder.AppendLine($"Person Cost[Euro/Person]: {PersonCost}<br/>");
            }
            return descriptionBuilder.ToString();
        }

        


    }
   
}