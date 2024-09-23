using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tech_project_back_end.Models
{
    public class Supplier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("supplier_id", TypeName = "varchar(36)")]
        public string SupplierId { get; set; }

        [Required]
        [Column("supplier_name", TypeName = "varchar(255)")]
        public string SupplierName { get; set; }
    }
}