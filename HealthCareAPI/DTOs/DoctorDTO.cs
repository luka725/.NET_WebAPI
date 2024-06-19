using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HealthCareAPI.DTOs
{
    public class DoctorDTO
    {
        public int DoctorID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Specialization { get; set; }

        public string ContactNumber { get; set; }

        public string Email { get; set; }
    }
}