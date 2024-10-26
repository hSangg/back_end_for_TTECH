using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security;

namespace tech_project_back_end.Models.User
{
    public class RolePermission
    {
        [Key]
        [Column("role_id", TypeName = "varchar(36)")]
        [StringLength(36)]
        public string RoleId { get; set; }

        [Key]
        [Column("permission_id", TypeName = "varchar(36)")]
        [StringLength(36)]
        public string PermissionId { get; set; }

        public virtual Role Role { get; set; }
        public virtual Permission Permission { get; set; }
    }
}