using tech_project_back_end.DTO.Discount;

namespace tech_project_back_end.Services.IService;

public interface IDiscountService
{
    Task<List<DiscountDTO>> GetAllDiscounts();
    
    Task<DiscountDTO> GetDiscountByCurrentDate(DateTime currentDate);
    
    Task<DiscountDTO> CreateDiscount(CreateDiscountDTO entity);
    
    Task<DiscountDTO> UpdateDiscount(DiscountDTO entity);
    
    Task DeleteDiscountById(string discountId);

}