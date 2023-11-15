using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace tech_project_back_end.Models
{
    public class UserLogin
    {
        [Required]
        [Column(TypeName = "varchar(11)")]
        [StringLength(11 | 10)]
        [RegularExpression(@"^0[1-9][0-9]{8}$", ErrorMessage = "Invalid phone number.")]
        public string phone { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        [StringLength(100)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string password { get; set; }

    }
}
