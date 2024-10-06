using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tech_project_back_end.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("user_id",TypeName = "varchar(36)")]
        [StringLength(36)]
        public string UserId { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "varchar(11)")]
        [StringLength(11 | 10)]
        public string Phone { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        [StringLength(100)]
        public string Password { get; set; }

        [Required]
        [Column("Role",TypeName = "varchar(50)")]
        [StringLength(50)]
        public string Role { get; set; }

        [Required]
        [Column("create_at")]
        public DateTime CreatedAt { get; set; }
    }
}
