namespace tech_project_back_end.DTO.Discount;

public class DiscountDTO
{
    public string DiscountId { get; set; }
    
    public string DiscountCode { get; set; }
    
    public int DiscountAmount { get; set; }
    
    public DateTime? DiscountDateFrom { get; set; }

    public DateTime DiscountDateTo { get; set; }
}