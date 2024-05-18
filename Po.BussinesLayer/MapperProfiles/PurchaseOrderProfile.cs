using AutoMapper;
using DataLayer.Models;
using Po.Common.Models.Dto;


namespace BussinesLayer.MapperProfiles
{
    public class PurchaseOrderProfile : Profile
    {
        public PurchaseOrderProfile()
        {
            CreateMap<PurchaseOrder, PurchaseOrderDto>().ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Companys.Name));
            CreateMap<PurchaseOrderDto, PurchaseOrder>();         
        }
    }
}
