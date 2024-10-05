using System.ComponentModel.DataAnnotations.Schema;
using tech_project_back_end.Models;

namespace tech_project_back_end.DTO.DetailOrder
{
    public class DetailOrderDTO
    {
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public long Price { get; set; }
        public int Quantity { get; set; }
    }
}
