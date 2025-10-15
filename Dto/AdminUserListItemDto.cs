using Plantpedia.Enum;

namespace Plantpedia.DTO;

public class AdminUserListItemDto
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string LastName { get; set; }
    public char Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string AvatarUrl { get; set; }
    public UserStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    // Stats
    public int CommentCount { get; set; }
    public int ReactionCount { get; set; }
    public int SearchCount { get; set; }
}
