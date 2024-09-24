using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tech_project_back_end.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("order_id", TypeName = "varchar(36)")]
        public string order_id { get; set; }

        [Column("user_id", TypeName = "varchar(36)")]
        public string user_id { get; set; }

        [ForeignKey("user_id")]
        public virtual User User { get; set; } 

        [Required]
        [Column("create_order_at", TypeName = "datetime")]
        public DateTime createdAt { get; set; }

        [Column("name", TypeName = "varchar(100)")]
        public string name { get; set; }

        [Column("email", TypeName = "varchar(100)")]
        public string email { get; set; }

        [Column("phone", TypeName = "varchar(20)")]
        public string phone { get; set; }

        [Column("address", TypeName = "varchar(200)")]
        public string address { get; set; }

        [Column("state", TypeName = "varchar(50)")]
        public string state { get; set; }

        [Column("note", TypeName = "varchar(500)")]
        public string note { get; set; }

        [Column("total", TypeName = "bigint(20)")]
        public long total { get; set; }

        [Column("discount", TypeName = "varchar(50)")]
        public string discount { get; set; }

        [ForeignKey("discount")]
        public virtual Discount Discount { get; set; }

        [Column("delivery_fee", TypeName = "int")]
        public int delivery_fee { get; set; }
    }
}
