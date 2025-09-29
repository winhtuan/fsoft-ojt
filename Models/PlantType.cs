using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plantpedia.Models
{
    [Table("plant_type")]
    public class PlantType
    {
        [Key]
        [Column("plant_type_id", TypeName = "char(10)")]
        public string PlantTypeId { get; set; }

        [Required]
        [Column("type_name")]
        [MaxLength(100)]
        public string TypeName { get; set; }

        [Column("description")]
        [MaxLength(255)]
        public string Description { get; set; }

        public ICollection<PlantInfo> Plants { get; set; }
    }
}
