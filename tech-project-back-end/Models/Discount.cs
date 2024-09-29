using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tech_project_back_end.Models
{
    public class Discount
    {
        [Key]
        [Column("discount_id", TypeName = "varchar(36)")]
        public string DiscountId { get; set; }

        [Column("discount_code", TypeName = "varchar(255)")]
        public string DiscountCode { get; set; }

        [Column("discount_amount", TypeName = "int(11)")]
        public int DiscountAmount { get; set; }

        [Column("discount_date_from", TypeName = "date")]
        public DateTime? DiscountDateFrom { get; set; }

        [Column("discount_date_to", TypeName = "datetime")]
        public DateTime DiscountDateTo { get; set; }
    }
}