using AutoMapper;
using tech_project_back_end.DTO.Order;
using tech_project_back_end.Models;

namespace tech_project_back_end.DTO
{
    public class MappingConfig : Profile
    {
        public MappingConfig() {
            CreateMap<Supplier, SupplierDTO>().ReverseMap();
            CreateMap <tech_project_back_end.Models.Order, OrderDTO >().ReverseMap();
        }
    }
}
