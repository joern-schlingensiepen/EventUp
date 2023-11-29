using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EventUpWebApp.Models
{
    public class ServiceViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
      
        public string Address { get; set; }
       
        public string Typ_Service { get; set; }
        public string Typ_Event { get; set; }
        public Nullable<int> Capacity { get; set; }
        public Nullable<double> FixCost { get; set; }
        public Nullable<double> HourCost { get; set; }
        public Nullable<double> PersonCost { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string City { get; set; }

        public string More { get; set; }
    }
}