//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EventUpLib
{
    using System;
    using System.Collections.Generic;
    
    public partial class Service
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Service()
        {
            this.isBookedFor = new HashSet<Event>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Typ_Service { get; set; }
        public string Typ_Event { get; set; }
        public Nullable<int> Capacity { get; set; }
        public Nullable<double> FixCost { get; set; }
        public Nullable<double> HourCost { get; set; }
        public Nullable<double> PersonCost { get; set; }
        public string City { get; set; }
        public string More { get; set; }
        public int isOfferedById { get; set; }
    
        public virtual User isOfferedBy { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Event> isBookedFor { get; set; }
    }
}
