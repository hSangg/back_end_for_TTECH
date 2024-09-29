using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tech_project_back_end.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("product_id", TypeName = "varchar(36)")]
        public string Product_id { get; set; }

        [Column("name_pr", TypeName = "varchar(200)")]
        public string name_pr { get; set; }

        [Column("name_serial", TypeName = "varchar(200)")]
        public string name_serial { get; set; }

        [Column("detail", TypeName = "longtext")]
        public string detail { get; set; }

        [Column("price", TypeName = "int(11)")]
        public ulong price { get; set; }

        [Column("quantity_pr", TypeName = "int(11)")]
        public int quantity_pr { get; set; }

        [Column("guarantee_period", TypeName = "int(11)")]
        public int guarantee_period { get; set; }

        [Column("supplier_id", TypeName = "varchar(36)")]
        [ForeignKey("Supplier")] 
        public string supplier_id { get; set; }

        public virtual Supplier Supplier { get; set; }

        public virtual ICollection<DetailOrder> DetailOrders { get; set; }

    }
}
