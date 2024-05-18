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
    public class ItemsProfile : Profile
    {
        public ItemsProfile()
        {
            CreateMap<Items, ItemsDto>().ForMember(dest=>dest.CompanyName, opt=> opt.MapFrom(src=>src.Companies.Name));
            CreateMap<ItemsDto, Items>();
        }
    }
}
