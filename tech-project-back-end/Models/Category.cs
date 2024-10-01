using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tech_project_back_end.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("category_id", TypeName = "varchar(36)")]
        public string CategoryId { get; set; } // Use Guid instead of string

        [Required]
        [Column("category_name", TypeName = "varchar(100)")]
        public string CategoryName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
