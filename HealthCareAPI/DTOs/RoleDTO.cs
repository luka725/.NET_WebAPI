using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HealthCareAPI.DTOs
{
    public class RoleDTO
    {
        public int RoleID { get; set; }

        public string RoleName { get; set; }
    }
}