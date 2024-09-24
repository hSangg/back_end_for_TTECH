using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tech_project_back_end.Models
{
    public class Cart
    {
        [Key]
        [Column("user_id", TypeName = "varchar(36)")]
        public string user_id { get; set; }

        [Key]
        [Column("product_id", TypeName = "varchar(36)")]
        public string product_id { get; set; }

        [Column("quantity", TypeName = "int(11)")]
        public int quantity { get; set; }

        [ForeignKey("user_id")]
        public virtual User User { get; set; } 

        [ForeignKey("product_id")]
        public virtual Product Product { get; set; } 
    }
}
