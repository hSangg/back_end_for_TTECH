using AutoMapper;
using tech_project_back_end.DTO.Order;
using tech_project_back_end.DTO.Cart;
using tech_project_back_end.DTO.Discount;
using tech_project_back_end.DTO.DetailOrder;
using tech_project_back_end.Models;
using tech_project_back_end.DTO.Users;
using tech_project_back_end.Models.User;

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

            CreateMap<User, UserDTO>().ReverseMap();

            CreateMap<User, UserLoginDTO>().ReverseMap();

            CreateMap<User, UserUpdateDTO>().ReverseMap();
            
            CreateMap<Models.DetailOrder, DetailOrderDTO>().ReverseMap();

            CreateMap<Models.Category, CategoryDTO>().ReverseMap();

            CreateMap<Models.Product, ProductDTO>().ReverseMap();

            CreateMap<Product, CreateProductDTO>().ReverseMap();
            
            CreateMap<Models.Image, ImageDTO>().ReverseMap();

            CreateMap<Image, CreateImageDTO>().ReverseMap();
        }
    }
}
