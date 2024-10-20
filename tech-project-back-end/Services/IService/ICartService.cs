using tech_project_back_end.DTO.Cart;
using tech_project_back_end.Models.ViewModel;

namespace tech_project_back_end.Services.IService;

public interface ICartService
{
    Task<int> GetUserTotalProduct(string user_id);
    
    Task<List<CartProductModel>> GetCartProduct(string user_id);

    Task<CartDTO> AddToCart(ModifyCartDTO entity, string user_id);
    
    Task<CartDTO> UpdateQuantity (ModifyCartDTO entity, string user_id);
    
    Task EmptyCart(string user_id);
}