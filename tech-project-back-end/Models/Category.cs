using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tech_project_back_end.Models
{
    [Keyless]
    public class Category
    {
        [Key]  // This attribute specifies that category_id is the primary key
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Use this if the ID is not an identity column
        public string category_id { get; set; }

        [Required]
        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string category_name { get; set; }
    }
}
