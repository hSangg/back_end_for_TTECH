using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tech_project_back_end.Models
{
    public class Detail_Order
    {
        [Key]
        [Column("order_id", TypeName = "varchar(36)")]
        public string OrderId { get; set; }

        [Key]
        [Column("product_id", TypeName = "varchar(36)")]
        public string ProductId { get; set; }

        [Column("price_pr", TypeName = "bigint(20) UNSIGNED")]
        public long PricePr { get; set; }

        [Column("quantity_pr", TypeName = "int(11)")]
        public int QuantityPr { get; set; }
    }
}