using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Plantpedia.Enum;
using Plantpedia.Models;

[Table("user_activity")]
public class UserActivity
{
    [Key]
    [Column("activity_id")]
    public long ActivityId { get; set; }

    [Required, Column("user_id")]
    public int UserId { get; set; }
    public UserAccount User { get; set; }

    [Required, Column("type")]
    public ActivityType Type { get; set; }

    [Column("ref_id")]
    public string? RefId { get; set; }

    [Column("metadata", TypeName = "jsonb")]
    public string? Metadata { get; set; }

    [Required, Column("created_at", TypeName = "timestamp with time zone")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
