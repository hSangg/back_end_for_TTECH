using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace tech_project_back_end.Models
{
    public class Permission
    {
        [Key]
        [Column("permission_id", TypeName = "varchar(50)")]
        [StringLength(50)]
        public string PermissionId { get; set; }

        [Column("permission_name", TypeName = "varchar(255)")]
        [StringLength(255)]
        public string PermissionName { get; set; }

        public virtual ICollection<RolePermission> RolePermissions
        {
            get; set;
        }
    }
}