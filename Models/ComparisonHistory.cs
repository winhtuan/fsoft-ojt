using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plantpedia.Models;

[Table("comparison_history")]
public class ComparisonHistory
{
    [Key]
    [Column("history_id")]
    public int HistoryId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("plant_id_1", TypeName = "char(10)")]
    public string PlantId1 { get; set; }

    [Column("plant_id_2", TypeName = "char(10)")]
    public string PlantId2 { get; set; }

    [Column("compared_at", TypeName = "timestamp with time zone")]
    public DateTime ComparedAt { get; set; } = DateTime.UtcNow;

    public UserAccount User { get; set; }
    public PlantInfo Plant1 { get; set; }
    public PlantInfo Plant2 { get; set; }
}
