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
    public class PurchaseOrderItemsProfile : Profile
    {
        public PurchaseOrderItemsProfile()
        {   
            CreateMap<PurchaseOrderItems, PurchaseOrderItemsDto>();
            CreateMap<PurchaseOrderItemsDto, PurchaseOrderItems>();
        }
    
    }
}
