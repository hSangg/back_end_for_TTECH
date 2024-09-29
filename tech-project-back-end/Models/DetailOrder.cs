using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tech_project_back_end.Models
{
    public class DetailOrder
    {
        [Key]
        [Column("order_id", TypeName = "varchar(36)")]
        public string OrderId { get; set; }

        [Key]
        [Column("product_id", TypeName = "varchar(36)")]
        public string ProductId { get; set; }

        [Column("price", TypeName = "bigint(20) UNSIGNED")]
        public long Price { get; set; }

        [Column("quantity", TypeName = "int(11)")]
        public int Quantity { get; set; }

        [ForeignKey("order_id")]
        public virtual Order Order { get; set; }

        [ForeignKey("product_id")]
        public virtual Product Product { get; set; }
    }
}
