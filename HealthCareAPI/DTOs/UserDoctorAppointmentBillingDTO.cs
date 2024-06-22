using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthCareAPI.DTOs
{
    public class UserDoctorAppointmentBillingDTO
    {
        public int? AppointmentID { get; set; }
        public int? PatientID { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientLastName { get; set; }
        public int? DoctorID { get; set; }
        public string DoctorFirstName { get; set; }
        public string DoctorLastName { get; set; }
        public DateTime? AppointmentDateTime { get; set; }
        public string AppointmentType { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public int? BillingID { get; set; }
        public DateTime? BillingDateTime { get; set; }
        public decimal? Amount { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
    }

}