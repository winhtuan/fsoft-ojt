using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plantpedia.Models;

[Table("discussion")]
public class Discussion
{
    [Key]
    [Column("discussion_id")]
    public int DiscussionId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("plant_id", TypeName = "char(10)")]
    public string? PlantId { get; set; }

    [Column("title")]
    [MaxLength(255)]
    public string Title { get; set; }

    [Column("content", TypeName = "text")]
    public string Content { get; set; }

    [Column("created_at", TypeName = "timestamp with time zone")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at", TypeName = "timestamp with time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("is_pinned")]
    public bool IsPinned { get; set; } = false;

    [Column("is_closed")]
    public bool IsClosed { get; set; } = false;

    [Column("view_count")]
    public int ViewCount { get; set; } = 0;

    public UserAccount User { get; set; }
    public PlantInfo? Plant { get; set; }
    public ICollection<DiscussionComment> Comments { get; set; } = new List<DiscussionComment>();
    public ICollection<DiscussionReaction> Reactions { get; set; } = new List<DiscussionReaction>();
}
