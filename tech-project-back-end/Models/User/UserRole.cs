using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tech_project_back_end.Models.User
{
    public class UserRole
    {
        [Key]
        [Column("user_id", TypeName = "varchar(36)")]
        public string UserId { get; set; }

        [Key]
        [Column("role_id", TypeName = "varchar(36)")]
        public string RoleId { get; set; }

        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}