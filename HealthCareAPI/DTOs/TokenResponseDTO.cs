using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthCareAPI.DTOs
{
    public class TokenResponseDTO
    {
        public string Token { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}