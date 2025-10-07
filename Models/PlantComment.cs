using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plantpedia.Models;

[Table("plant_comment")]
public class PlantComment
{
    [Key]
    [Column("comment_id")]
    public int CommentId { get; set; }

    [Required]
    [Column("plant_id", TypeName = "char(10)")]
    public string PlantId { get; set; }

    [Required]
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("parent_comment_id")]
    public int? ParentCommentId { get; set; }

    [Required]
    [Column("content", TypeName = "text")]
    public string Content { get; set; }

    [Column("created_at", TypeName = "timestamp with time zone")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at", TypeName = "timestamp with time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;

    // Navigation
    public UserAccount User { get; set; }
    public PlantInfo Plant { get; set; }

    [ForeignKey("ParentCommentId")]
    public PlantComment? ParentComment { get; set; }

    public ICollection<PlantComment> Replies { get; set; } = new List<PlantComment>();
    public ICollection<PlantCommentReaction> Reactions { get; set; } =
        new List<PlantCommentReaction>();
}
