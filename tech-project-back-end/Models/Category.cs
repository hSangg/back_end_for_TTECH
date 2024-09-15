using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tech_project_back_end.Models
{
    [Keyless]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string category_id { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string category_name { get; set; }
    }
}
