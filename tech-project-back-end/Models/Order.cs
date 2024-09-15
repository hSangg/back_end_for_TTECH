using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tech_project_back_end.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("order_id", TypeName = "varchar(36)")]
        public string OrderId { get; set; }

        [Column("user_id", TypeName = "varchar(36)")]
        public string UserId { get; set; }

        [Required]
        [Column("create_order_at", TypeName = "datetime")]
        public DateTime CreateOrderAt { get; set; }

        [Column("name", TypeName = "varchar(100)")]
        public string Name { get; set; }

        [Column("email", TypeName = "varchar(100)")]
        public string Email { get; set; }

        [Column("phone", TypeName = "varchar(20)")]
        public string Phone { get; set; }

        [Column("address", TypeName = "varchar(200)")]
        public string Address { get; set; }

        [Column("state", TypeName = "varchar(50)")]
        public string State { get; set; }

        [Column("note", TypeName = "varchar(500)")]
        public string Note { get; set; }

        [Column("total", TypeName = "bigint(20)")]
        public long Total { get; set; }

        [Column("discount", TypeName = "varchar(50)")]
        public string Discount { get; set; }

        [Column("delivery_fee", TypeName = "int")]
        public int DeliveryFee { get; set; }
    }
}