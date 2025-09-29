using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plantpedia.Models
{
    [Table("region")]
    public class Region
    {
        [Key]
        [Column("region_id", TypeName = "char(10)")]
        public string RegionId { get; set; }

        [Required]
        [Column("name")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Column("note")]
        [MaxLength(255)]
        public string Note { get; set; }

        public ICollection<PlantRegion> PlantRegions { get; set; }
    }
}
