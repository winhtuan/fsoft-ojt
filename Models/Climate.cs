using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plantpedia.Models
{
    [Table("climate")]
    public class Climate
    {
        [Key]
        [Column("climate_id", TypeName = "char(10)")]
        public string ClimateId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; }

        [MaxLength(50)]
        [Column("temperature_range")]
        public string TemperatureRange { get; set; }

        [MaxLength(50)]
        [Column("rainfall_range")]
        public string RainfallRange { get; set; }

        [MaxLength(50)]
        [Column("humidity_range")]
        public string HumidityRange { get; set; }

        public ICollection<PlantClimate> PlantClimates { get; set; }
    }
}
