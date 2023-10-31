using Microsoft.EntityFrameworkCore;
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

            [Required]
            [Column("name_pr", TypeName = "varchar(200)")]
            public string NamePr { get; set; }

            [Required]
            [Column("name_serial", TypeName = "varchar(200)")]
            public string NameSerial { get; set; }


            [Required]
            [Column("detail", TypeName = "longtext")]
            public string Detail { get; set; }

            [Required]
            [Column("price", TypeName = "int(11)")]
            public int Price { get; set; }

            [Required]
            [Column("quantity_pr", TypeName = "int(11)")]
            public int QuantityPr { get; set; }

            [Required]
            [Column("img_id", TypeName = "varchar(36)")]
            public string ImgId { get; set; }

            [Required]
            [Column("guarantee_period", TypeName = "int(11)")]
            public int GuaranteePeriod { get; set; }

            [Required]
            [Column("supplier_id", TypeName = "varchar(36)")]
            public string SupplierId { get; set; }
        }
}
