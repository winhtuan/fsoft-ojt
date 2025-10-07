using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plantpedia.Models
{
    [Table("plant_info")]
    public class PlantInfo
    {
        [Key]
        [Column("plant_id", TypeName = "char(10)")]
        public string PlantId { get; set; }

        [Required]
        [Column("scientific_name")]
        [MaxLength(255)]
        public string ScientificName { get; set; }

        [Column("common_name")]
        [MaxLength(255)]
        public string CommonName { get; set; }

        [Column("description", TypeName = "text")]
        public string Description { get; set; }

        [Column("plant_type_id", TypeName = "char(10)")]
        public string PlantTypeId { get; set; }
        public PlantType PlantType { get; set; }
        public PlantCare CareInfo { get; set; }
        public ICollection<PlantRegion> PlantRegions { get; set; }
        public ICollection<PlantSoil> PlantSoils { get; set; }
        public ICollection<PlantClimate> PlantClimates { get; set; }
        public ICollection<PlantUsage> PlantUsages { get; set; }
        public ICollection<PlantImg> PlantImages { get; set; }

        [Column("created_date", TypeName = "timestamp with time zone")]
        public DateTime? CreatedDate { get; set; }

        [Column("updated_date", TypeName = "timestamp with time zone")]
        public DateTime? UpdatedDate { get; set; }

        [Column("harvest_date_days")]
        public int? HarvestDate { get; set; }

        public ICollection<UserFavorite> FavoritedBy { get; set; } = new List<UserFavorite>();
        public ICollection<UserFavorite> UserFavorites { get; set; } = new List<UserFavorite>();
        public ICollection<PlantComment> Comments { get; set; } = new List<PlantComment>();
    }
}
