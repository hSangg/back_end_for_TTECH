using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tech_project_back_end.Models
{
    public class Detail_Order
    {
        [Key]
        [Column("order_id", TypeName = "varchar(36)")]
        public string order_id { get; set; }

        [Key]
        [Column("product_id", TypeName = "varchar(36)")]
        public string product_id { get; set; }

        [Column("price", TypeName = "bigint(20) UNSIGNED")]
        public long price { get; set; }

        [Column("quality", TypeName = "int(11)")]
        public int quality { get; set; }
    }
}