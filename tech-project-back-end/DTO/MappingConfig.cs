using AutoMapper;
using tech_project_back_end.DTO.Discount;
using tech_project_back_end.Models;

namespace tech_project_back_end.DTO
{
    public class MappingConfig : Profile
    {
        public MappingConfig() {
            CreateMap<Supplier, SupplierDTO>().ReverseMap();

            CreateMap<Image, ImageDTO>().ReverseMap();
            
            CreateMap<Models.Discount, DiscountDTO>().ReverseMap();
            
            CreateMap<Models.Discount, CreateDiscountDTO>().ReverseMap();

        }
    }
}
