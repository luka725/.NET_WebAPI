namespace HealthCareDAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Appointment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]

        public Appointment()
        {
            Billings = new HashSet<Billing>();
        }

        public int AppointmentID { get; set; }

        public int? PatientID { get; set; }

        public int? DoctorID { get; set; }

        public DateTime? AppointmentDateTime { get; set; }

        [StringLength(100)]
        public string AppointmentType { get; set; }

        [StringLength(20)]
        public string Status { get; set; }

        public string Notes { get; set; }

        public virtual Doctor Doctor { get; set; }

        public virtual Patient Patient { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Billing> Billings { get; set; }
    }
}
