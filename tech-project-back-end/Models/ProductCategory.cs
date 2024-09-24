using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tech_project_back_end.Models
{
    public class ProductCategory
    {
        [Key]
        [Column("product_id", TypeName = "varchar(36)")]
        public string product_id { get; set; }

        [Key]
        [Column("category_id", TypeName = "varchar(36)")]
        public string category_id { get; set; }
            
        [ForeignKey("product_id")]
        public virtual Product Product { get; set; } 

        [ForeignKey("category_id")]
        public virtual Category Category { get; set; } 
    }
}
