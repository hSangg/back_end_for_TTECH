using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace tech_project_back_end.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] 
        [Column(TypeName = "varchar(36)")]
        [StringLength(36)]
        public string user_id { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        [StringLength(100)]
        public string name { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        [StringLength(100)]
        public string email { get; set; }

        [Required]
        [Column(TypeName = "varchar(11)")]
        [StringLength(11 | 10)]
        public string phone { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        [StringLength(100)]
        public string password { get; set; }

        [Required]
        [Column(TypeName = "varchar(1)")]
        [StringLength(1)]
        public string isAdmin { get; set; }

        [Required]
        public DateTime create_at { get; set; }
    }
}
