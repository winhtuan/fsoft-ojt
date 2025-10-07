using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plantpedia.Models;

[Table("discussion_comment")]
public class DiscussionComment
{
    [Key]
    [Column("comment_id")]
    public int CommentId { get; set; }

    [Column("discussion_id")]
    public int DiscussionId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("parent_comment_id")]
    public int? ParentCommentId { get; set; }

    [Column("content", TypeName = "text")]
    public string Content { get; set; }

    [Column("created_at", TypeName = "timestamp with time zone")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at", TypeName = "timestamp with time zone")]
    public DateTime? UpdatedAt { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;

    public UserAccount User { get; set; }
    public Discussion Discussion { get; set; }
    public DiscussionComment? ParentComment { get; set; }
    public ICollection<DiscussionComment> Replies { get; set; } = new List<DiscussionComment>();
    public ICollection<DiscussionReaction> Reactions { get; set; } = new List<DiscussionReaction>();
}
