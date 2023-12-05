using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
        [Display(Name = "Typ Service")]
        public string Typ_Service { get; set; }

        // Lista de opciones para Typ_Service
        public List<SelectListItem> TypServiceOptions { get; set; }
        public string Typ_Event { get; set; }
        public List<SelectListItem> TypEventOptions { get; set; }
        
        public Nullable<int> Capacity { get; set; }
        public Nullable<double> FixCost { get; set; }
        public Nullable<double> HourCost { get; set; }
        public Nullable<double> PersonCost { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        public string More { get; set; }
        public int isOfferedById { get; set; }
    }
   
}