using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plantpedia.Models;

[Table("plant_comment_reaction")]
public class PlantCommentReaction
{
    [Key]
    [Column("reaction_id")]
    public int ReactionId { get; set; }

    [Column("comment_id")]
    public int CommentId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("reaction_type", TypeName = "char(1)")]
    public char ReactionType { get; set; } // 'L' = Like, 'D' = Dislike

    [Column("created_at", TypeName = "timestamp with time zone")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public PlantComment Comment { get; set; }
    public UserAccount User { get; set; }
}
