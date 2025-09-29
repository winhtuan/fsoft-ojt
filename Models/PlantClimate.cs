using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plantpedia.Models
{
    [Table("plant_climate")]
    public class PlantClimate
    {
        [Key, Column("plant_id", TypeName = "char(10)")]
        public string PlantId { get; set; }
        public PlantInfo Plant { get; set; }

        [Key, Column("climate_id", TypeName = "char(10)")]
        public string ClimateId { get; set; }
        public Climate Climate { get; set; }
    }
}
