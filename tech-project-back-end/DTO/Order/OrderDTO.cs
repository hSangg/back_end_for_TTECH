using tech_project_back_end.Models;

namespace tech_project_back_end.DTO.Order
{
    public class OrderDTO
    {
        public string order_id { get; set; }
        public string user_id { get; set; }
        public User User { get; set; }
        public DateTime createdAt { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string state { get; set; }
        public string note { get; set; }
        public long total { get; set; }
        public string discount_id { get; set; }
        public Models.Discount Discount { get; set; }
        public int delivery_fee { get; set; }

    }
}
