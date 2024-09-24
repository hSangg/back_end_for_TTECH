using System.ComponentModel.DataAnnotations.Schema;

namespace tech_project_back_end.Models
{
    public class BaseEntity
    {
        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;
        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [Column("modified_date")]
        public DateTime? ModifiedDate { get; set; }
    }
}
