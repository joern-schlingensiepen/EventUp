using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EventUpWebApp.Models
{
    public class EventViewModel
    {
        [DisplayName("When Starts?")] // Cambia el nombre de visualización aquí
        public DateTime Start_DateTime { get; set; }


        [DisplayName("When Ends?")] // Cambia el nombre de visualización aquí
        public DateTime End_DateTime { get; set; }


        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Address { get; set; }

        [DisplayName("Number of guest")]
        public int NumberOfGuest { get; set; }

        [DisplayName("Budget [Euro]")]
        public Nullable<double> Budget { get; set; }
        public string Typ_Event { get; set; }
        public List<SelectListItem> TypEventOptions { get; set; }
        //public System.DateTime Start_DateTime { get; set; }
        //public System.DateTime End_DateTime { get; set; }
        public int isPlannedById { get; set; }

     
    }
}