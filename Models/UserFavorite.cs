using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Plantpedia.Models;

namespace Plantpedia.Models;

[Table("user_favorite")]
public class UserFavorite
{
    [Key]
    [Column("favorite_id")]
    public int FavoriteId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("plant_id", TypeName = "char(10)")]
    public string PlantId { get; set; }

    [Column("created_at", TypeName = "timestamp with time zone")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("note")]
    [MaxLength(255)]
    public string? Note { get; set; }

    public UserAccount User { get; set; }
    public PlantInfo Plant { get; set; }
}
