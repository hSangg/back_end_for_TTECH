using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tech_project_back_end.Models
{
    public class Product_Category
    {
        [Key]
        [Column("product_id", TypeName = "varchar(36)")]
        public string ProductId { get; set; }

        [Key]
        [Column("category_id", TypeName = "varchar(36)")]
        public string CategoryId { get; set; }

    }
}