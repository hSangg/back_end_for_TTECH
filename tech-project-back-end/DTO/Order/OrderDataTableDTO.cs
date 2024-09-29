using tech_project_back_end.Models;

namespace tech_project_back_end.DTO.Order
{
    public class OrderDataTableDTO
    {
        public string orderId { get; set; }
        public string customerName { get; set; }
        public string customerEmail { get; set; }
        public string customerPhone { get; set; }
        public string customerAddress { get; set; }
        public string customerNote { get; set; }
        public long total { get; set; }
        public string discountId { get; set; }
        public DateTime createdAt { get; set; }
        public int deliveryFee { get; set; }
    }
}
