using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plantpedia.Models
{
    [Table("user_login_data")]
    public class UserLoginData
    {
        [Key]
        [ForeignKey("User")]
        [Column("user_id")]
        public int UserId { get; set; }
        public UserAccount User { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("username")]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("password_salt")]
        public string PasswordSalt { get; set; }

        [Required]
        [MaxLength(250)]
        [Column("password_hash")]
        public string PasswordHash { get; set; }

        [Required]
        [Column("created_at", TypeName = "timestamp with time zone")]
        public DateTime CreatedAt { get; set; }

        [Column("last_login_at", TypeName = "timestamp with time zone")]
        public DateTime? LastLoginAt { get; set; }
    }
}
