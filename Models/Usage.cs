using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plantpedia.Models
{
    [Table("usage")]
    public class Usage
    {
        [Key]
        [Column("usage_id", TypeName = "char(10)")]
        public string UsageId { get; set; }

        [Required]
        [Column("name")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Column("note")]
        [MaxLength(255)]
        public string Note { get; set; }

        public ICollection<PlantUsage> PlantUsages { get; set; }
    }
}
