using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tech_project_back_end.Models
{
    public class Image
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string image_id { get; set; }

        [Column(TypeName = "varchar(36)")]
        public string product_id { get; set; }

        [Column("image_href", TypeName = "varchar(250)")]
        public string image_href { get; set; }

        [Column("file_name", TypeName = "varchar(250)")]
        public string file_name { get; set; }

    }
}
