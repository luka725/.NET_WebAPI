using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HealthCareAPI.DTOs
{
    public class AppointmentDTO
    {
        public int AppointmentID { get; set; }

        public int? PatientID { get; set; }

        public int? DoctorID { get; set; }

        public DateTime? AppointmentDateTime { get; set; }

        public string AppointmentType { get; set; }

        public string Status { get; set; }

        public string Notes { get; set; }
    }
}