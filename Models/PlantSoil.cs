using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plantpedia.Models
{
    [Table("plant_soil")]
    public class PlantSoil
    {
        [Key, Column("plant_id", TypeName = "char(10)")]
        public string PlantId { get; set; }
        public PlantInfo Plant { get; set; }

        [Key, Column("soil_type_id", TypeName = "char(10)")]
        public string SoilTypeId { get; set; }
        public SoilType SoilType { get; set; }
    }
}
