using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plantpedia.Models
{
    [Table("plant_comparison_item")]
    public class PlantComparisonItem
    {
        [Column("comparison_id")]
        public int ComparisonId { get; set; }

        [Column("plant_id", TypeName = "char(10)")]
        public string PlantId { get; set; }

        [Column("sort_order")]
        public int SortOrder { get; set; } = 0;

        [MaxLength(255)]
        [Column("note")]
        public string? Note { get; set; }

        // Navigation
        public PlantComparison Comparison { get; set; }
        public PlantInfo Plant { get; set; }
    }
}
