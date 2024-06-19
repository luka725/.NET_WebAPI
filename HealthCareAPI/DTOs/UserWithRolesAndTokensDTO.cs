﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthCareAPI.DTOs
{
    public class UserWithRolesAndTokensDTO
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; }
        public RoleDTO Roles { get; set; }
        public ICollection<TokenDTO> Token { get; set; }
    }
}