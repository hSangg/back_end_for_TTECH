namespace tech_project_back_end.DTO.Order
{
    public class OrderDTO
    {
        public string orderId { get; set; }
        public string userId { get; set; }
        public DateTime createdAt { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string state { get; set; }
        public string note { get; set; }
        public long total { get; set; }
        public string? discountId { get; set; }
        public int deliveryFee { get; set; }
    }
}
