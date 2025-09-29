using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plantpedia.Models
{
    [Table("plant_region")]
    public class PlantRegion
    {
        [Key, Column("plant_id", TypeName = "char(10)")]
        public string PlantId { get; set; }
        public PlantInfo Plant { get; set; }

        [Key, Column("region_id", TypeName = "char(10)")]
        public string RegionId { get; set; }
        public Region Region { get; set; }
    }
}
