using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tech_project_back_end.Models
{
    public class Image
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ImageId { get; set; }

        [Column("product_id", TypeName = "varchar(36)")]
        public string ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } 

        [Column("image_href", TypeName = "varchar(250)")]
        public string ImageHref { get; set; }

        [Column("file_name", TypeName = "varchar(250)")]
        public string FileName { get; set; }
    }
}
