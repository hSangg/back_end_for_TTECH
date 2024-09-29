using AutoMapper;
using tech_project_back_end.DTO.Order;
using tech_project_back_end.DTO.Cart;
using tech_project_back_end.DTO.Discount;
using tech_project_back_end.Models;

namespace tech_project_back_end.DTO
{
    public class MappingConfig : Profile
    {
        public MappingConfig() {

            CreateMap<Supplier, SupplierDTO>().ReverseMap();

            CreateMap <Models.Order, OrderDTO >().ReverseMap();

            CreateMap<Models.Discount, DiscountDTO>().ReverseMap();

            CreateMap<Models.Discount, CreateDiscountDTO>().ReverseMap();

            CreateMap<Models.Cart, CartDTO>().ReverseMap();
            
        }
    }
}
