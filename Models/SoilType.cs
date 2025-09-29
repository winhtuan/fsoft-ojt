using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plantpedia.Models
{
    [Table("soil_type")]
    public class SoilType
    {
        [Key]
        [Column("soil_type_id", TypeName = "char(10)")]
        public string SoilTypeId { get; set; }

        [Required]
        [Column("name")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Column("note")]
        [MaxLength(255)]
        public string Note { get; set; }

        public ICollection<PlantSoil> PlantSoils { get; set; }
    }
}
