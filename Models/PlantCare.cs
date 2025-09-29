using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Plantpedia.Enum;

namespace Plantpedia.Models
{
    [Table("plant_care")]
    public class PlantCare
    {
        [Key]
        [Column("plant_id", TypeName = "char(10)")]
        public string PlantId { get; set; }

        [Required]
        [Column("light_requirement")]
        public LightRequirement LightRequirement { get; set; }

        [Required]
        [Column("watering_needs")]
        public WateringNeeds WateringNeeds { get; set; }

        [Column("humidity_preference")]
        public HumidityPreference? HumidityPreference { get; set; }

        [Column("growth_rate")]
        public GrowthRate? GrowthRate { get; set; }

        [Column("soil_recommendation", TypeName = "text")]
        public string SoilRecommendation { get; set; }

        [Column("fertilizer_info", TypeName = "text")]
        public string FertilizerInfo { get; set; }

        public PlantInfo PlantInfo { get; set; }
    }
}
