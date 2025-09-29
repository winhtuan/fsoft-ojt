using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plantpedia.Models
{
    [Table("plant_img")]
    public class PlantImg
    {
        [Key]
        [Column("image_id", TypeName = "char(10)")]
        public string ImageId { get; set; }

        [Column("plant_id", TypeName = "char(10)")]
        public string PlantId { get; set; }
        public PlantInfo Plant { get; set; }

        [Required]
        [Column("image_url")]
        public string ImageUrl { get; set; }

        [Column("caption")]
        [MaxLength(255)]
        public string Caption { get; set; }
    }
}
