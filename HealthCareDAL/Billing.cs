namespace HealthCareDAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Billing")]
    public partial class Billing
    {
        public int BillingID { get; set; }

        public int? PatientID { get; set; }

        public int? DoctorID { get; set; }

        public int? AppointmentID { get; set; }

        public DateTime? BillingDateTime { get; set; }

        public decimal? Amount { get; set; }

        [StringLength(20)]
        public string PaymentStatus { get; set; }

        [StringLength(50)]
        public string PaymentMethod { get; set; }

        public virtual Appointment Appointment { get; set; }

        public virtual Doctor Doctor { get; set; }

        public virtual Patient Patient { get; set; }
    }
}
