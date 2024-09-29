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

        [ForeignKey("user_id")]
        public virtual User User { get; set; }

        [Required]
        [Column("create_order_at", TypeName = "timestamp")]
        public DateTime CreatedAt { get; set; }

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
        public string note { get; set; }

        [Column("total", TypeName = "bigint(20)")]
        public long Total { get; set; }

        [Column("discount_id", TypeName = "varchar(50)")]
        public string DiscountId { get; set; }

        [ForeignKey("discount_id")]
        public virtual Discount Discount { get; set; }

        [Column("delivery_fee", TypeName = "int")]
        public int DeliveryFee { get; set; }
        public virtual ICollection<DetailOrder> DetailOrders { get; set; }

    }
}
