using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Plantpedia.Enum;

namespace Plantpedia.Models
{
    [Table("user_account")]
    public class UserAccount
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required, MaxLength(255)]
        [Column("last_name")]
        public string LastName { get; set; }

        [Required, Column("gender", TypeName = "char(1)")]
        public char Gender { get; set; }

        [Required, Column("date_of_birth", TypeName = "timestamp with time zone")]
        public DateTime DateOfBirth { get; set; }

        [Column("avatar_url")]
        public string AvatarUrl { get; set; }

        [Required, Column("status")]
        public UserStatus Status { get; set; } = UserStatus.Active;

        [Required, Column("created_at", TypeName = "timestamp with time zone")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at", TypeName = "timestamp with time zone")]
        public DateTime? UpdatedAt { get; set; }

        [Column("deleted_at", TypeName = "timestamp with time zone")]
        public DateTime? DeletedAt { get; set; }

        public UserLoginData LoginData { get; set; }
        public ICollection<UserFavorite> Favorites { get; set; } = new List<UserFavorite>();
        public ICollection<PlantComment> PlantComments { get; set; } = new List<PlantComment>();
        public ICollection<PlantCommentReaction> PlantCommentReactions { get; set; } =
            new List<PlantCommentReaction>();
        public ICollection<UserActivity> Activities { get; set; } = new List<UserActivity>();
    }
}
