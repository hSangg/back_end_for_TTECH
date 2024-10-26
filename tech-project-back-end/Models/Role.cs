using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace tech_project_back_end.Models
{
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("role_id", TypeName = "varchar(36)")]
        [StringLength(36)]
        public string RoleId { get; set; }

        [Required]
        [Column("role_name", TypeName = "varchar(100)")]
        [StringLength(100)]

        public string RoleName { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; }

    }
}
