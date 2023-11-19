using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace EventUpWebApp.Models
{
    public class EventModel
    {
        [DisplayName("When Starts?")] // Cambia el nombre de visualización aquí
        public DateTime Start_DateTime { get; set; }


        [DisplayName("When Ends?")] // Cambia el nombre de visualización aquí
        public DateTime End_DateTime { get; set; }

    }
}