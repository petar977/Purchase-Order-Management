using AutoMapper;
using DataLayer.Models;
using Po.Common.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinesLayer.MapperProfiles
{
    public class OrderCountManagerProfile : Profile
    {
        public OrderCountManagerProfile()
        {
            CreateMap<OrderCount, OrderCountDto>().ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name));
            CreateMap<OrderCountDto, OrderCount>();
        }
    }
}
