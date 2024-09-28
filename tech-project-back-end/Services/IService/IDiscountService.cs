using tech_project_back_end.DTO.Discount;

namespace tech_project_back_end.Services.IService;

public interface IDiscountService
{
    Task<List<DiscountDTO>> GetAllDiscountsAsync();
    
    Task<DiscountDTO> GetDiscountByCurrentDateAsync(DateTime currentDate);
    
    Task<DiscountDTO> CreateDiscountAsync(CreateDiscountDTO entity);
    
    Task<DiscountDTO> UpdateDiscountAsync(DiscountDTO entity);
    
    Task DeleteDiscountByIdAsync(string discountId);

}