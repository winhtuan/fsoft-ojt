using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plantpedia.Models;

[Table("plant_comparison")]
public class PlantComparison
{
    [Key]
    [Column("comparison_id")]
    public int ComparisonId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("name")]
    [MaxLength(255)]
    public string Name { get; set; }

    [Column("description", TypeName = "text")]
    public string? Description { get; set; }

    [Column("created_at", TypeName = "timestamp with time zone")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at", TypeName = "timestamp with time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("is_public")]
    public bool IsPublic { get; set; } = false;

    public UserAccount User { get; set; }
    public ICollection<PlantComparisonItem> Items { get; set; } = new List<PlantComparisonItem>();
}
