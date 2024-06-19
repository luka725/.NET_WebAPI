using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HealthCareAPI.DTOs
{
    public class TokenDTO
    {
        public int TokenID { get; set; }

        public int? UserID { get; set; }

        public string TokenValue { get; set; }

        public DateTime? ExpiryDateTime { get; set; }

        public string TokenType { get; set; }
    }
}