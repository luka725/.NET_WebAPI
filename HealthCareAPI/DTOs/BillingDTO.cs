using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HealthCareAPI.DTOs
{
    public class BillingDTO
    {
        public int BillingID { get; set; }

        public int? PatientID { get; set; }

        public int? DoctorID { get; set; }

        public int? AppointmentID { get; set; }

        public DateTime? BillingDateTime { get; set; }

        public decimal? Amount { get; set; }

        public string PaymentStatus { get; set; }

        public string PaymentMethod { get; set; }
    }
}