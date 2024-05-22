using AutoMapper;
using DataAccesLayer;
using DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace WebApplication.Common
{
    public static class AutoMapperConfig
    {
        private static readonly IMapper _mapper;

        static AutoMapperConfig()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UsersDTO>();
                cfg.CreateMap<UsersDTO, User>();
                cfg.CreateMap<Category, CategoriesDTO>();
                cfg.CreateMap<CategoriesDTO, Category>();
                cfg.CreateMap<Discount, DiscountsDTO>();
                cfg.CreateMap<DiscountsDTO, Discount>();
                cfg.CreateMap<OrderDetail, OrderDetailsDTO>();
                cfg.CreateMap<OrderDetailsDTO, OrderDetail>();
                cfg.CreateMap<Order, OrdersDTO>();
                cfg.CreateMap<OrdersDTO, Order>();
                cfg.CreateMap<ProductsDiscount, ProductsDiscountsDTO>();
                cfg.CreateMap<ProductsDiscountsDTO, ProductsDiscount>();
                cfg.CreateMap<Product, ProductsDTO>();
                cfg.CreateMap<ProductsDTO, Product>();
                cfg.CreateMap<Role, RolesDTO>();
                cfg.CreateMap<RolesDTO, Role>();

            });
            _mapper = config.CreateMapper();
        }
        public static IMapper Mapper => _mapper;
    }
}