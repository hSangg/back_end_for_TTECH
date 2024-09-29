using AutoMapper;
<<<<<<< HEAD
using tech_project_back_end.DTO.Order;
=======
using tech_project_back_end.DTO.Discount;
>>>>>>> main
using tech_project_back_end.Models;

namespace tech_project_back_end.DTO
{
    public class MappingConfig : Profile
    {
        public MappingConfig() {
            CreateMap<Supplier, SupplierDTO>().ReverseMap();
<<<<<<< HEAD
            CreateMap <tech_project_back_end.Models.Order, OrderDTO >().ReverseMap();
=======
            
            CreateMap<Models.Discount, DiscountDTO>().ReverseMap();
            
            CreateMap<Models.Discount, CreateDiscountDTO>().ReverseMap();
>>>>>>> main
        }
    }
}
