using AutoMapper;
using HealthCareAPI.DTOs;
using HealthCareDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthCareAPI.Common
{
    public static class AutoMapperConfig
    {
        private static readonly IMapper _mapper;

        static AutoMapperConfig()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<UserDTO, User>();
                cfg.CreateMap<Role, RoleDTO>();
                cfg.CreateMap<RoleDTO, Role>();
                cfg.CreateMap<Appointment, AppointmentDTO>();
                cfg.CreateMap<AppointmentDTO, Appointment>();
                cfg.CreateMap<Billing, BillingDTO>();
                cfg.CreateMap<BillingDTO, Billing>();
                cfg.CreateMap<Doctor, DoctorDTO>();
                cfg.CreateMap<DoctorDTO, Doctor>();
                cfg.CreateMap<Patient, PatientDTO>();
                cfg.CreateMap<PatientDTO, Patient>();
                cfg.CreateMap<Token, TokenDTO>();
                cfg.CreateMap<TokenDTO, Token>();
                cfg.CreateMap<User, UserWithRolesDTO>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Role));
                cfg.CreateMap<User, UserWithRolesAndTokensDTO>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Role))
                .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Tokens));
            });
            _mapper = config.CreateMapper();
        }
        public static IMapper Mapper => _mapper;
    }
}