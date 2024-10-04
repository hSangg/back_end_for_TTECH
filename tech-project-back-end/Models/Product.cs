using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tech_project_back_end.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("product_id", TypeName = "varchar(36)")]
        public string ProductId { get; set; }

        [Column("name_pr", TypeName = "varchar(200)")]
        public string NamePr { get; set; }

        [Column("name_serial", TypeName = "varchar(200)")]
        public string NameSerial { get; set; }

        [Column("detail", TypeName = "longtext")]
        public string Detail { get; set; }

        [Column("price", TypeName = "int(11)")]
        public ulong Price { get; set; }

        [Column("quantity_pr", TypeName = "int(11)")]
        public int QuantityPr { get; set; }

        [Column("guarantee_period", TypeName = "int(11)")]
        public int GuaranteePeriod { get; set; }

        [Column("supplier_id", TypeName = "varchar(36)")]
        [ForeignKey("Supplier")] 
        public string SupplierId { get; set; }

        [Column("category_id", TypeName = "varchar(36)")]
        [ForeignKey("Category")]
        public string CategoryId { get; set; }

        public virtual Supplier Supplier { get; set; }

        public virtual Category Category{ get; set; }

        public virtual ICollection<DetailOrder> DetailOrders { get; set; }

    }
}
