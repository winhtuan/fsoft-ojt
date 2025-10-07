using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Plantpedia.Models;

namespace Plantpedia.Models;

[Table("discussion_reaction")]
public class DiscussionReaction
{
    [Key]
    [Column("reaction_id")]
    public int ReactionId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("discussion_id")]
    public int? DiscussionId { get; set; }

    [Column("comment_id")]
    public int? CommentId { get; set; }

    [Column("reaction_type", TypeName = "char(1)")]
    public char ReactionType { get; set; } // 'L' hoặc 'D'

    [Column("created_at", TypeName = "timestamp with time zone")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public UserAccount User { get; set; }
    public Discussion? Discussion { get; set; }
    public DiscussionComment? Comment { get; set; }
}
